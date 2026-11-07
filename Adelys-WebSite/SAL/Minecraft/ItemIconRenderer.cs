using System.Numerics;
using System.Text.Json.Nodes;

namespace Adelys_WebSite.SAL.Minecraft
{
    /// <summary>
    /// Dessine l'icône d'inventaire d'un item à partir des modèles et textures du client Minecraft :
    /// les items plats reprennent leurs calques, les blocs sont rendus en 3D avec la vue « gui » de leur modèle
    /// </summary>
    public class ItemIconRenderer
    {
        public const int IconSize = 128;
        private const int Supersampling = 2;

        private readonly Func<string, Stream?> _openAsset;
        private readonly Dictionary<string, ResolvedModel?> _models = new();
        private readonly Dictionary<string, RgbaImage?> _textures = new();

        /// <param name="openAsset">Ouvre un fichier du client à partir de son chemin, par exemple assets/minecraft/models/block/stone.json</param>
        public ItemIconRenderer(Func<string, Stream?> openAsset)
        {
            _openAsset = openAsset;
        }

        /// <summary>
        /// Rend la définition d'un item (le contenu de « model » dans assets/minecraft/items), null si l'item est dessiné par le moteur du jeu
        /// </summary>
        public RgbaImage? RenderItem(JsonNode itemModel)
        {
            List<ModelPart> parts = new();
            CollectParts(itemModel, Vector3.Zero, parts);
            return RenderParts(parts);
        }

        /// <summary>
        /// Rend directement un modèle, pour les versions sans dossier items où chaque item a son modèle item/&lt;id&gt;
        /// </summary>
        public RgbaImage? RenderModel(string modelReference)
        {
            return RenderParts(new List<ModelPart> { new ModelPart(modelReference, new List<Vector3?>(), Vector3.Zero) });
        }

        private RgbaImage? RenderParts(List<ModelPart> parts)
        {
            List<(ModelPart Part, ResolvedModel Model)> resolved = parts
                .Select(p => (p, Resolve(p.Model)))
                .Where(p => p.Item2 != null)
                .Select(p => (p.p, p.Item2!))
                .ToList();
            if (resolved.Count == 0)
            {
                return null;
            }

            (ModelPart flatPart, ResolvedModel flatModel) = resolved.FirstOrDefault(p => p.Model.Generated);
            RgbaImage? icon = flatModel != null
                ? RenderFlat(flatModel, flatPart.Tints)
                : RenderBlock(resolved.Where(p => p.Model.Elements != null).ToList());
            return icon != null && HasVisiblePixel(icon) ? icon : null;
        }

        private void CollectParts(JsonNode? node, Vector3 offset, List<ModelPart> parts)
        {
            if (node == null)
            {
                return;
            }

            JsonNode? translation = node["transformation"]?["translation"];
            if (translation is JsonArray)
            {
                offset += ReadVector(translation, Vector3.Zero) * 16;
            }

            switch (node["type"]?.GetValue<string>())
            {
                case "minecraft:model":
                    List<Vector3?> tints = (node["tints"] as JsonArray)?.Select(ReadTint).ToList() ?? new();
                    parts.Add(new ModelPart(node["model"]!.GetValue<string>(), tints, offset));
                    break;
                case "minecraft:composite":
                    foreach (JsonNode? child in node["models"] as JsonArray ?? new JsonArray())
                    {
                        CollectParts(child, offset, parts);
                    }
                    break;
                // Pour un item qui change d'apparence, on garde son aspect par défaut
                case "minecraft:select":
                    CollectParts(node["fallback"] ?? node["cases"]?[0]?["model"], offset, parts);
                    break;
                case "minecraft:condition":
                    CollectParts(node["on_false"], offset, parts);
                    break;
                case "minecraft:range_dispatch":
                    CollectParts(node["fallback"] ?? node["entries"]?[0]?["model"], offset, parts);
                    break;
            }
        }

        private Vector3? ReadTint(JsonNode? tint)
        {
            switch (tint?["type"]?.GetValue<string>())
            {
                case "minecraft:constant":
                    return ArgbToColor(tint["value"]!.GetValue<int>());
                case "minecraft:grass":
                    return SampleColormap("grass", tint["temperature"]?.GetValue<float>() ?? 0.5f, tint["downfall"]?.GetValue<float>() ?? 1f);
                default:
                    JsonNode? fallback = tint?["default"];
                    return fallback != null ? ArgbToColor(fallback.GetValue<int>()) : null;
            }
        }

        private Vector3? SampleColormap(string name, float temperature, float downfall)
        {
            RgbaImage? colormap = LoadTexture($"colormap/{name}", cropAnimation: false);
            if (colormap == null)
            {
                return null;
            }
            temperature = Math.Clamp(temperature, 0, 1);
            downfall = Math.Clamp(downfall, 0, 1) * temperature;
            int x = Math.Clamp((int)((1 - temperature) * 255), 0, colormap.Width - 1);
            int y = Math.Clamp((int)((1 - downfall) * 255), 0, colormap.Height - 1);
            int o = (y * colormap.Width + x) * 4;
            return new Vector3(colormap.Pixels[o], colormap.Pixels[o + 1], colormap.Pixels[o + 2]) / 255f;
        }

        private static Vector3 ArgbToColor(int argb)
        {
            return new Vector3((argb >> 16) & 0xFF, (argb >> 8) & 0xFF, argb & 0xFF) / 255f;
        }

        private RgbaImage RenderFlat(ResolvedModel model, List<Vector3?> tints)
        {
            RgbaImage icon = new RgbaImage(IconSize, IconSize);
            for (int layer = 0; model.Textures.ContainsKey($"layer{layer}"); layer++)
            {
                RgbaImage? texture = ResolveTexture(model, $"#layer{layer}");
                if (texture == null)
                {
                    continue;
                }
                Vector3? tint = layer < tints.Count ? tints[layer] : null;
                for (int y = 0; y < IconSize; y++)
                {
                    for (int x = 0; x < IconSize; x++)
                    {
                        int o = ((y * texture.Height / IconSize) * texture.Width + x * texture.Width / IconSize) * 4;
                        Blend(icon.Pixels, (y * IconSize + x) * 4, texture.Pixels, o, tint ?? Vector3.One);
                    }
                }
            }
            return icon;
        }

        private RgbaImage RenderBlock(List<(ModelPart Part, ResolvedModel Model)> parts)
        {
            int size = IconSize * Supersampling;
            RgbaImage canvas = new RgbaImage(size, size);
            float[] depth = Enumerable.Repeat(float.NegativeInfinity, size * size).ToArray();
            ResolvedModel main = parts[0].Model;
            Matrix4x4 view = BuildGuiTransform(main);

            List<Face> faces = new();
            foreach ((ModelPart part, ResolvedModel model) in parts)
            {
                foreach (JsonNode? element in model.Elements!)
                {
                    if (element != null)
                    {
                        AddElementFaces(element, model, part, view, faces);
                    }
                }
            }

            // Des faces les plus éloignées aux plus proches, pour que les textures translucides se superposent correctement
            foreach (Face face in faces.OrderBy(f => f.Corners.Average(c => c.Z)))
            {
                DrawFace(face, canvas, depth, size);
            }
            return Downsample(canvas, Supersampling);
        }

        private static Matrix4x4 BuildGuiTransform(ResolvedModel model)
        {
            Vector3 rotation = model.GuiRotation * (MathF.PI / 180);
            // Même ordre que le jeu : rotation X, puis Y, puis Z, appliquées au modèle centré sur l'origine
            Matrix4x4 rotate = Matrix4x4.CreateRotationZ(rotation.Z) * Matrix4x4.CreateRotationY(rotation.Y) * Matrix4x4.CreateRotationX(rotation.X);
            return Matrix4x4.CreateTranslation(-8, -8, -8)
                * Matrix4x4.CreateScale(model.GuiScale)
                * rotate
                * Matrix4x4.CreateTranslation(model.GuiTranslation);
        }

        private void AddElementFaces(JsonNode element, ResolvedModel model, ModelPart part, Matrix4x4 view, List<Face> faces)
        {
            Vector3 from = ReadVector(element["from"], Vector3.Zero);
            Vector3 to = ReadVector(element["to"], new Vector3(16));
            bool shade = element["shade"]?.GetValue<bool>() ?? true;
            Matrix4x4 elementRotation = ReadElementRotation(element["rotation"]);

            foreach ((string direction, JsonNode? faceNode) in element["faces"] as JsonObject ?? new JsonObject())
            {
                if (faceNode == null || ResolveTexture(model, faceNode["texture"]?.GetValue<string>()) is not RgbaImage texture)
                {
                    continue;
                }

                Vector3[] corners = FaceCorners(direction, from, to);
                if (corners.Length == 0)
                {
                    continue;
                }
                corners = corners.Select(c => Vector3.Transform(Vector3.Transform(c, elementRotation) + part.Offset, view)).ToArray();

                // Face tournée vers l'arrière : invisible, comme dans le jeu
                Vector3 normal = Vector3.Cross(corners[3] - corners[0], corners[1] - corners[0]);
                if (normal.Z <= 0)
                {
                    continue;
                }

                Vector4 uv = ReadUv(faceNode["uv"], direction, from, to);
                int uvRotation = ((faceNode["rotation"]?.GetValue<int>() ?? 0) / 90 % 4 + 4) % 4;
                int tintIndex = faceNode["tintindex"]?.GetValue<int>() ?? -1;
                Vector3 tint = tintIndex >= 0 && tintIndex < part.Tints.Count ? part.Tints[tintIndex] ?? Vector3.One : Vector3.One;
                float light = shade && model.GuiLight != "front" ? ScreenShade(Vector3.Normalize(normal)) : 1f;

                faces.Add(new Face(corners, CornerUvs(uv, uvRotation), texture, tint * light));
            }
        }

        /// <summary>
        /// Éclairage de l'inventaire : le dessus est le plus clair, puis le côté gauche, puis le côté droit
        /// </summary>
        private static float ScreenShade(Vector3 normal)
        {
            if (normal.Y > 0.5f)
            {
                return 1f;
            }
            if (normal.Y < -0.5f)
            {
                return 0.5f;
            }
            return normal.X < 0 ? 0.8f : 0.6f;
        }

        /// <summary>
        /// Coins d'une face vus de l'extérieur, dans l'ordre haut gauche, haut droite, bas droite, bas gauche
        /// </summary>
        private static Vector3[] FaceCorners(string direction, Vector3 f, Vector3 t)
        {
            return direction switch
            {
                "up" => new[] { new Vector3(f.X, t.Y, f.Z), new Vector3(t.X, t.Y, f.Z), new Vector3(t.X, t.Y, t.Z), new Vector3(f.X, t.Y, t.Z) },
                "down" => new[] { new Vector3(f.X, f.Y, t.Z), new Vector3(t.X, f.Y, t.Z), new Vector3(t.X, f.Y, f.Z), new Vector3(f.X, f.Y, f.Z) },
                "north" => new[] { new Vector3(t.X, t.Y, f.Z), new Vector3(f.X, t.Y, f.Z), new Vector3(f.X, f.Y, f.Z), new Vector3(t.X, f.Y, f.Z) },
                "south" => new[] { new Vector3(f.X, t.Y, t.Z), new Vector3(t.X, t.Y, t.Z), new Vector3(t.X, f.Y, t.Z), new Vector3(f.X, f.Y, t.Z) },
                "west" => new[] { new Vector3(f.X, t.Y, f.Z), new Vector3(f.X, t.Y, t.Z), new Vector3(f.X, f.Y, t.Z), new Vector3(f.X, f.Y, f.Z) },
                "east" => new[] { new Vector3(t.X, t.Y, t.Z), new Vector3(t.X, t.Y, f.Z), new Vector3(t.X, f.Y, f.Z), new Vector3(t.X, f.Y, t.Z) },
                _ => Array.Empty<Vector3>()
            };
        }

        /// <summary>
        /// UV par défaut du jeu : la portion de texture qui correspond à la position de la face dans le bloc
        /// </summary>
        private static Vector4 ReadUv(JsonNode? uv, string direction, Vector3 f, Vector3 t)
        {
            if (uv is JsonArray values && values.Count == 4)
            {
                return new Vector4(values[0]!.GetValue<float>(), values[1]!.GetValue<float>(), values[2]!.GetValue<float>(), values[3]!.GetValue<float>());
            }
            return direction switch
            {
                "up" => new Vector4(f.X, f.Z, t.X, t.Z),
                "down" => new Vector4(f.X, 16 - t.Z, t.X, 16 - f.Z),
                "north" => new Vector4(16 - t.X, 16 - t.Y, 16 - f.X, 16 - f.Y),
                "south" => new Vector4(f.X, 16 - t.Y, t.X, 16 - f.Y),
                "west" => new Vector4(f.Z, 16 - t.Y, t.Z, 16 - f.Y),
                _ => new Vector4(16 - t.Z, 16 - t.Y, 16 - f.Z, 16 - f.Y)
            };
        }

        /// <summary>
        /// Le jeu numérote les coins haut gauche, bas gauche, bas droite, haut droite et décale ce numéro d'un cran par quart de tour
        /// </summary>
        private static Vector2[] CornerUvs(Vector4 uv, int rotation)
        {
            Vector2[] gameOrder = { new(uv.X, uv.Y), new(uv.X, uv.W), new(uv.Z, uv.W), new(uv.Z, uv.Y) };
            int[] gameIndex = { 0, 3, 2, 1 };
            return gameIndex.Select(i => gameOrder[(i + rotation) % 4]).ToArray();
        }

        private static Matrix4x4 ReadElementRotation(JsonNode? rotation)
        {
            if (rotation == null)
            {
                return Matrix4x4.Identity;
            }

            Vector3 origin = ReadVector(rotation["origin"], new Vector3(8));
            Matrix4x4 rotate = Matrix4x4.Identity;
            if (rotation["axis"] != null)
            {
                float angle = (rotation["angle"]?.GetValue<float>() ?? 0) * MathF.PI / 180;
                string axis = rotation["axis"]!.GetValue<string>();
                rotate = axis switch
                {
                    "x" => Matrix4x4.CreateRotationX(angle),
                    "y" => Matrix4x4.CreateRotationY(angle),
                    _ => Matrix4x4.CreateRotationZ(angle)
                };
                // rescale garde la taille apparente de l'élément une fois tourné
                if (rotation["rescale"]?.GetValue<bool>() == true && MathF.Cos(angle) > 0.01f)
                {
                    float scale = 1 / MathF.Cos(angle);
                    Vector3 rescale = axis switch { "x" => new(1, scale, scale), "y" => new(scale, 1, scale), _ => new(scale, scale, 1) };
                    rotate = Matrix4x4.CreateScale(rescale) * rotate;
                }
            }
            else
            {
                Vector3 angles = new Vector3(rotation["x"]?.GetValue<float>() ?? 0, rotation["y"]?.GetValue<float>() ?? 0, rotation["z"]?.GetValue<float>() ?? 0) * (MathF.PI / 180);
                rotate = Matrix4x4.CreateRotationX(angles.X) * Matrix4x4.CreateRotationY(angles.Y) * Matrix4x4.CreateRotationZ(angles.Z);
            }
            return Matrix4x4.CreateTranslation(-origin) * rotate * Matrix4x4.CreateTranslation(origin);
        }

        /// <summary>
        /// Projection orthographique : 16 unités du modèle occupent toute l'icône, la caméra regarde vers -Z
        /// </summary>
        private static void DrawFace(Face face, RgbaImage canvas, float[] depth, int size)
        {
            Vector2[] screen = face.Corners.Select(c => new Vector2((c.X + 8) / 16 * size, (8 - c.Y) / 16 * size)).ToArray();
            Vector2 origin = screen[0];
            Vector2 right = screen[1] - origin;
            Vector2 down = screen[3] - origin;
            float determinant = right.X * down.Y - right.Y * down.X;
            if (MathF.Abs(determinant) < 1e-4f)
            {
                return;
            }

            int minX = Math.Max(0, (int)MathF.Floor(screen.Min(p => p.X)));
            int maxX = Math.Min(size - 1, (int)MathF.Ceiling(screen.Max(p => p.X)));
            int minY = Math.Max(0, (int)MathF.Floor(screen.Min(p => p.Y)));
            int maxY = Math.Min(size - 1, (int)MathF.Ceiling(screen.Max(p => p.Y)));
            RgbaImage texture = face.Texture;

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Vector2 d = new Vector2(x + 0.5f, y + 0.5f) - origin;
                    float s = (d.X * down.Y - d.Y * down.X) / determinant;
                    float t = (right.X * d.Y - right.Y * d.X) / determinant;
                    if (s < 0 || s >= 1 || t < 0 || t >= 1)
                    {
                        continue;
                    }

                    float z = face.Corners[0].Z + s * (face.Corners[1].Z - face.Corners[0].Z) + t * (face.Corners[3].Z - face.Corners[0].Z);
                    int index = y * size + x;
                    if (z < depth[index] - 1e-3f)
                    {
                        continue;
                    }

                    Vector2 uv = face.Uvs[0] + s * (face.Uvs[1] - face.Uvs[0]) + t * (face.Uvs[3] - face.Uvs[0]);
                    int tx = Math.Clamp((int)MathF.Floor(uv.X / 16 * texture.Width), 0, texture.Width - 1);
                    int ty = Math.Clamp((int)MathF.Floor(uv.Y / 16 * texture.Height), 0, texture.Height - 1);
                    int texel = (ty * texture.Width + tx) * 4;
                    byte alpha = texture.Pixels[texel + 3];
                    if (alpha == 0)
                    {
                        continue;
                    }

                    Blend(canvas.Pixels, index * 4, texture.Pixels, texel, face.Color);
                    if (alpha == 255)
                    {
                        depth[index] = z;
                    }
                }
            }
        }

        private static void Blend(byte[] target, int t, byte[] source, int s, Vector3 color)
        {
            float alpha = source[s + 3] / 255f;
            if (alpha <= 0)
            {
                return;
            }
            float targetAlpha = target[t + 3] / 255f;
            float outAlpha = alpha + targetAlpha * (1 - alpha);
            for (int c = 0; c < 3; c++)
            {
                float sourceValue = source[s + c] * color[c];
                float mixed = (sourceValue * alpha + target[t + c] * targetAlpha * (1 - alpha)) / outAlpha;
                target[t + c] = (byte)Math.Clamp(MathF.Round(mixed), 0, 255);
            }
            target[t + 3] = (byte)Math.Clamp(MathF.Round(outAlpha * 255), 0, 255);
        }

        private static RgbaImage Downsample(RgbaImage source, int factor)
        {
            RgbaImage result = new RgbaImage(source.Width / factor, source.Height / factor);
            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    float r = 0, g = 0, b = 0, a = 0;
                    for (int sy = 0; sy < factor; sy++)
                    {
                        for (int sx = 0; sx < factor; sx++)
                        {
                            int o = ((y * factor + sy) * source.Width + x * factor + sx) * 4;
                            float alpha = source.Pixels[o + 3];
                            r += source.Pixels[o] * alpha;
                            g += source.Pixels[o + 1] * alpha;
                            b += source.Pixels[o + 2] * alpha;
                            a += alpha;
                        }
                    }
                    int target = (y * result.Width + x) * 4;
                    if (a > 0)
                    {
                        result.Pixels[target] = (byte)(r / a);
                        result.Pixels[target + 1] = (byte)(g / a);
                        result.Pixels[target + 2] = (byte)(b / a);
                        result.Pixels[target + 3] = (byte)(a / (factor * factor));
                    }
                }
            }
            return result;
        }

        private static bool HasVisiblePixel(RgbaImage image)
        {
            for (int i = 3; i < image.Pixels.Length; i += 4)
            {
                if (image.Pixels[i] > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static Vector3 ReadVector(JsonNode? node, Vector3 fallback)
        {
            return node is JsonArray values && values.Count == 3
                ? new Vector3(values[0]!.GetValue<float>(), values[1]!.GetValue<float>(), values[2]!.GetValue<float>())
                : fallback;
        }

        /// <summary>
        /// Fusionne un modèle avec ses parents : les textures s'additionnent, les éléments et la vue gui viennent du plus proche qui les définit
        /// </summary>
        private ResolvedModel? Resolve(string reference)
        {
            string path = StripNamespace(reference);
            if (_models.TryGetValue(path, out ResolvedModel? cached))
            {
                return cached;
            }

            ResolvedModel? model = null;
            if (path == "builtin/generated")
            {
                model = new ResolvedModel { Generated = true };
            }
            else if (ReadJson($"assets/minecraft/models/{path}.json") is JsonObject json)
            {
                ResolvedModel parent = json["parent"] != null ? Resolve(json["parent"]!.GetValue<string>()) ?? new ResolvedModel() : new ResolvedModel();
                model = new ResolvedModel
                {
                    Generated = parent.Generated,
                    Textures = new Dictionary<string, string>(parent.Textures),
                    Elements = json["elements"] as JsonArray ?? parent.Elements,
                    GuiRotation = parent.GuiRotation,
                    GuiTranslation = parent.GuiTranslation,
                    GuiScale = parent.GuiScale,
                    GuiLight = json["gui_light"]?.GetValue<string>() ?? parent.GuiLight
                };
                foreach ((string key, JsonNode? value) in json["textures"] as JsonObject ?? new JsonObject())
                {
                    // Une texture peut aussi être un objet { sprite, force_translucent }
                    string? texture = value is JsonObject sprite ? sprite["sprite"]?.GetValue<string>() : value?.GetValue<string>();
                    if (texture != null)
                    {
                        model.Textures[key] = texture;
                    }
                }
                if (json["display"]?["gui"] is JsonObject gui)
                {
                    model.GuiRotation = ReadVector(gui["rotation"], Vector3.Zero);
                    model.GuiTranslation = ReadVector(gui["translation"], Vector3.Zero);
                    model.GuiScale = ReadVector(gui["scale"], Vector3.One);
                }
            }
            _models[path] = model;
            return model;
        }

        private RgbaImage? ResolveTexture(ResolvedModel model, string? reference)
        {
            for (int depth = 0; reference != null && reference.StartsWith('#') && depth < 16; depth++)
            {
                reference = model.Textures.GetValueOrDefault(reference[1..]);
            }
            return reference == null || reference.StartsWith('#') ? null : LoadTexture(StripNamespace(reference), cropAnimation: true);
        }

        private RgbaImage? LoadTexture(string path, bool cropAnimation)
        {
            if (_textures.TryGetValue(path, out RgbaImage? cached))
            {
                return cached;
            }

            RgbaImage? texture = null;
            using (Stream? stream = _openAsset($"assets/minecraft/textures/{path}.png"))
            {
                if (stream != null)
                {
                    texture = RgbaImage.ReadPng(stream);
                }
            }
            // Une texture animée empile ses images verticalement, l'icône garde la première
            if (texture != null && cropAnimation && texture.Height > texture.Width && texture.Height % texture.Width == 0)
            {
                RgbaImage frame = new RgbaImage(texture.Width, texture.Width);
                Array.Copy(texture.Pixels, frame.Pixels, frame.Pixels.Length);
                texture = frame;
            }
            _textures[path] = texture;
            return texture;
        }

        private JsonNode? ReadJson(string path)
        {
            using Stream? stream = _openAsset(path);
            return stream != null ? JsonNode.Parse(stream) : null;
        }

        private static string StripNamespace(string reference)
        {
            return reference.StartsWith("minecraft:") ? reference["minecraft:".Length..] : reference;
        }

        private record ModelPart(string Model, List<Vector3?> Tints, Vector3 Offset);

        private record Face(Vector3[] Corners, Vector2[] Uvs, RgbaImage Texture, Vector3 Color);

        private class ResolvedModel
        {
            public bool Generated { get; set; }
            public Dictionary<string, string> Textures { get; set; } = new();
            public JsonArray? Elements { get; set; }
            public Vector3 GuiRotation { get; set; } = Vector3.Zero;
            public Vector3 GuiTranslation { get; set; } = Vector3.Zero;
            public Vector3 GuiScale { get; set; } = Vector3.One;
            public string GuiLight { get; set; } = "side";
        }
    }
}
