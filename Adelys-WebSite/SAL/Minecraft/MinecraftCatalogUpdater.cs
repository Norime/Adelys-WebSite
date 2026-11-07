using System.Globalization;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Adelys_WebSite.SAL.Minecraft
{
    /// <summary>
    /// Tient le catalogue à jour sans intervention : au démarrage puis toutes les 12 heures, il regarde la version du serveur
    /// et, si elle a changé, télécharge le client Minecraft officiel pour en tirer les noms français et les icônes
    /// </summary>
    public partial class MinecraftCatalogUpdater : BackgroundService
    {
        private const string ManifestUrl = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
        private const string ResourcesUrl = "https://resources.download.minecraft.net";
        private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(12);

        private readonly MinecraftItemCatalog _catalog;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MinecraftCatalogUpdater> _logger;
        private readonly HttpClient _http;

        public MinecraftCatalogUpdater(MinecraftItemCatalog catalog, IConfiguration configuration, ILogger<MinecraftCatalogUpdater> logger, IHttpClientFactory httpClientFactory)
        {
            _catalog = catalog;
            _configuration = configuration;
            _logger = logger;
            _http = httpClientFactory.CreateClient();
            _http.Timeout = TimeSpan.FromMinutes(10);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LoadLatestBuiltCatalog();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Update(stoppingToken);
                }
                catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError(ex, "Impossible de mettre à jour le catalogue des items Minecraft, nouvel essai dans {Interval}", CheckInterval);
                }
                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        /// <summary>
        /// Le site affiche tout de suite le dernier catalogue construit, même si Mojang ou le serveur ne répondent pas
        /// </summary>
        private void LoadLatestBuiltCatalog()
        {
            string? latest = Directory.GetDirectories(_catalog.RootPath)
                .Select(Path.GetFileName)
                .Where(v => v != null && _catalog.IsBuilt(v))
                .OrderByDescending(v => File.GetLastWriteTimeUtc(Path.Combine(_catalog.GetVersionPath(v!), MinecraftItemCatalog.CatalogFileName)))
                .FirstOrDefault();
            if (latest != null)
            {
                _catalog.Load(latest);
            }
        }

        private async Task Update(CancellationToken cancellationToken)
        {
            JsonNode manifest = (await GetJson(ManifestUrl, cancellationToken))!;
            string version = await ResolveVersion(manifest, cancellationToken);
            if (_catalog.Version == version)
            {
                return;
            }

            if (!_catalog.IsBuilt(version))
            {
                _logger.LogInformation("Construction du catalogue des items Minecraft {Version}", version);
                await Build(version, manifest, cancellationToken);
            }
            _catalog.Load(version);
            _logger.LogInformation("Catalogue des items Minecraft {Version} chargé : {Count} items", version, _catalog.Items.Count);
            DeleteOtherVersions(version);
        }

        /// <summary>
        /// Version imposée dans la configuration, sinon celle annoncée par le serveur, sinon la dernière version publiée
        /// </summary>
        private async Task<string> ResolveVersion(JsonNode manifest, CancellationToken cancellationToken)
        {
            HashSet<string> knownVersions = manifest["versions"]!.AsArray().Select(v => v!["id"]!.GetValue<string>()).ToHashSet();

            string? configured = _configuration["Minecraft:Version"];
            if (!string.IsNullOrWhiteSpace(configured))
            {
                return configured;
            }

            string? address = _configuration["Minecraft:ServerAddress"];
            if (!string.IsNullOrWhiteSpace(address))
            {
                try
                {
                    // Un serveur Paper ou Spigot annonce par exemple « Paper 1.21.4 » : on garde le numéro connu de Mojang
                    string? name = await MinecraftServerPing.GetVersionName(address, cancellationToken);
                    string? version = name == null ? null : VersionPattern().Matches(name).Select(m => m.Value).FirstOrDefault(knownVersions.Contains);
                    if (version != null)
                    {
                        return version;
                    }
                    _logger.LogWarning("Version du serveur Minecraft non reconnue : {Name}", name);
                }
                catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "Le serveur Minecraft {Address} ne répond pas, le catalogue suit la dernière version publiée", address);
                }
            }

            return manifest["latest"]!["release"]!.GetValue<string>();
        }

        private async Task Build(string version, JsonNode manifest, CancellationToken cancellationToken)
        {
            string versionUrl = manifest["versions"]!.AsArray().First(v => v!["id"]!.GetValue<string>() == version)!["url"]!.GetValue<string>();
            JsonNode versionInfo = (await GetJson(versionUrl, cancellationToken))!;
            JsonObject frenchNames = await GetFrenchNames(versionInfo, cancellationToken);

            string buildPath = _catalog.GetVersionPath(version) + ".building";
            string jarPath = buildPath + ".jar";
            if (Directory.Exists(buildPath))
            {
                Directory.Delete(buildPath, recursive: true);
            }
            string iconsPath = Path.Combine(buildPath, MinecraftItemCatalog.IconsFolder);
            Directory.CreateDirectory(iconsPath);

            try
            {
                await using (FileStream jar = File.Create(jarPath))
                {
                    await using Stream download = await _http.GetStreamAsync(versionInfo["downloads"]!["client"]!["url"]!.GetValue<string>(), cancellationToken);
                    await download.CopyToAsync(jar, cancellationToken);
                }

                List<MinecraftItem> items;
                using (ZipArchive client = ZipFile.OpenRead(jarPath))
                {
                    items = BuildItems(client, frenchNames, iconsPath, cancellationToken);
                }

                await File.WriteAllTextAsync(Path.Combine(buildPath, MinecraftItemCatalog.CatalogFileName), JsonSerializer.Serialize(items), cancellationToken);
                Directory.Move(buildPath, _catalog.GetVersionPath(version));
            }
            finally
            {
                File.Delete(jarPath);
                if (Directory.Exists(buildPath))
                {
                    Directory.Delete(buildPath, recursive: true);
                }
            }
        }

        private List<MinecraftItem> BuildItems(ZipArchive client, JsonObject frenchNames, string iconsPath, CancellationToken cancellationToken)
        {
            JsonObject englishNames = ReadJson(client, "assets/minecraft/lang/en_us.json") as JsonObject ?? new JsonObject();
            ItemIconRenderer renderer = new ItemIconRenderer(path => OpenEntry(client, path));
            List<MinecraftItem> items = new();

            // Depuis la 1.21.4, chaque item a sa définition dans assets/minecraft/items, avant il fallait passer par la langue et les modèles
            List<ZipArchiveEntry> definitions = client.Entries.Where(e => e.FullName.StartsWith("assets/minecraft/items/") && e.FullName.EndsWith(".json")).ToList();
            IEnumerable<string> ids = definitions.Count > 0
                ? definitions.Select(e => Path.GetFileNameWithoutExtension(e.Name))
                : englishNames.Select(n => n.Key)
                    .Select(k => ItemKeyPattern().Match(k))
                    .Where(m => m.Success && client.GetEntry($"assets/minecraft/models/item/{m.Groups[1].Value}.json") != null)
                    .Select(m => m.Groups[1].Value)
                    .Distinct();

            foreach (string id in ids.Where(i => i != "air"))
            {
                cancellationToken.ThrowIfCancellationRequested();
                RgbaImage? icon = null;
                try
                {
                    icon = definitions.Count > 0
                        ? renderer.RenderItem(ReadJson(client, $"assets/minecraft/items/{id}.json")!["model"]!)
                        : renderer.RenderModel($"item/{id}");
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Icône impossible à dessiner pour {Id}", id);
                }

                if (icon != null)
                {
                    using FileStream file = File.Create(Path.Combine(iconsPath, id + ".png"));
                    icon.WritePng(file);
                }
                items.Add(new MinecraftItem(id, FindName(id, frenchNames, englishNames), icon != null));
            }

            CompareInfo french = CultureInfo.GetCultureInfo("fr-FR").CompareInfo;
            items.Sort((a, b) => french.Compare(a.Name, b.Name, CompareOptions.IgnoreCase));
            return items;
        }

        /// <summary>
        /// Les disques, modèles de forge et motifs de bannière partagent un même nom : on y ajoute leur description, comme dans le jeu
        /// </summary>
        private static string FindName(string id, JsonObject frenchNames, JsonObject englishNames)
        {
            string name = FindTranslation(new[] { $"item.minecraft.{id}", $"block.minecraft.{id}" }, frenchNames, englishNames) ?? id;

            List<string> descriptionKeys = new() { $"item.minecraft.{id}.desc" };
            if (id.EndsWith("_armor_trim_smithing_template"))
            {
                descriptionKeys.Add($"trim_pattern.minecraft.{id[..^"_armor_trim_smithing_template".Length]}");
            }
            else if (id.EndsWith("_smithing_template"))
            {
                descriptionKeys.Add($"upgrade.minecraft.{id[..^"_smithing_template".Length]}");
            }
            string? description = FindTranslation(descriptionKeys, frenchNames, englishNames);
            return description != null ? $"{name} ({description})" : name;
        }

        private static string? FindTranslation(IEnumerable<string> keys, JsonObject frenchNames, JsonObject englishNames)
        {
            foreach (JsonObject names in new[] { frenchNames, englishNames })
            {
                foreach (string key in keys)
                {
                    if (names[key] is JsonValue translation)
                    {
                        return translation.GetValue<string>();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Le français n'est pas dans le client : il se trouve dans les ressources de la version, retrouvées par leur empreinte
        /// </summary>
        private async Task<JsonObject> GetFrenchNames(JsonNode versionInfo, CancellationToken cancellationToken)
        {
            JsonNode? assetIndex = await GetJson(versionInfo["assetIndex"]!["url"]!.GetValue<string>(), cancellationToken);
            string? hash = assetIndex?["objects"]?["minecraft/lang/fr_fr.json"]?["hash"]?.GetValue<string>();
            if (hash == null)
            {
                return new JsonObject();
            }
            return await GetJson($"{ResourcesUrl}/{hash[..2]}/{hash}", cancellationToken) as JsonObject ?? new JsonObject();
        }

        private void DeleteOtherVersions(string version)
        {
            foreach (string directory in Directory.GetDirectories(_catalog.RootPath))
            {
                if (Path.GetFileName(directory) != version)
                {
                    try
                    {
                        Directory.Delete(directory, recursive: true);
                    }
                    catch (IOException ex)
                    {
                        _logger.LogWarning(ex, "Ancien catalogue impossible à supprimer : {Directory}", directory);
                    }
                }
            }
        }

        private async Task<JsonNode?> GetJson(string url, CancellationToken cancellationToken)
        {
            await using Stream stream = await _http.GetStreamAsync(url, cancellationToken);
            return await JsonNode.ParseAsync(stream, cancellationToken: cancellationToken);
        }

        private static JsonNode? ReadJson(ZipArchive client, string path)
        {
            using Stream? stream = OpenEntry(client, path);
            return stream != null ? JsonNode.Parse(stream) : null;
        }

        private static Stream? OpenEntry(ZipArchive client, string path)
        {
            ZipArchiveEntry? entry = client.GetEntry(path);
            if (entry == null)
            {
                return null;
            }
            MemoryStream copy = new MemoryStream();
            using (Stream stream = entry.Open())
            {
                stream.CopyTo(copy);
            }
            copy.Position = 0;
            return copy;
        }

        [GeneratedRegex(@"\d+(?:\.\d+)+")]
        private static partial Regex VersionPattern();

        [GeneratedRegex(@"^(?:item|block)\.minecraft\.([a-z0-9_]+)$")]
        private static partial Regex ItemKeyPattern();
    }
}
