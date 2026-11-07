using Adelys_WebSite.SAL.Interface;
using System.Text.Json;

namespace Adelys_WebSite.SAL.Minecraft
{
    /// <summary>
    /// Catalogue des items de la version jouée sur le serveur, reconstruit par MinecraftCatalogUpdater quand la version change
    /// </summary>
    public class MinecraftItemCatalog : IMinecraftItemCatalog
    {
        public const string CatalogFileName = "catalog.json";
        public const string IconsFolder = "icons";
        public const string IconsRequestPath = "/minecraft";

        private CatalogSnapshot _snapshot = new(null, new List<MinecraftItem>(), new Dictionary<string, MinecraftItem>());

        public string RootPath { get; }

        public string? Version => _snapshot.Version;
        public IReadOnlyList<MinecraftItem> Items => _snapshot.Items;

        public MinecraftItemCatalog(IConfiguration configuration, IWebHostEnvironment environment)
        {
            string storagePath = configuration["Minecraft:StoragePath"] ?? "App_Data/Minecraft";
            RootPath = Path.GetFullPath(storagePath, environment.ContentRootPath);
            Directory.CreateDirectory(RootPath);
        }

        public MinecraftItem? Find(string? id)
        {
            return id != null ? _snapshot.ById.GetValueOrDefault(id) : null;
        }

        public string? GetIconUrl(string? id)
        {
            CatalogSnapshot snapshot = _snapshot;
            return id != null && snapshot.ById.TryGetValue(id, out MinecraftItem? item) && item.HasIcon
                ? $"{IconsRequestPath}/{Uri.EscapeDataString(snapshot.Version!)}/{IconsFolder}/{Uri.EscapeDataString(id)}.png"
                : null;
        }

        public string GetVersionPath(string version)
        {
            return Path.Combine(RootPath, version);
        }

        public bool IsBuilt(string version)
        {
            return File.Exists(Path.Combine(GetVersionPath(version), CatalogFileName));
        }

        /// <summary>
        /// Remplace le catalogue en une fois : une page en cours de rendu garde l'ancien jusqu'au bout
        /// </summary>
        public void Load(string version)
        {
            List<MinecraftItem> items = JsonSerializer.Deserialize<List<MinecraftItem>>(File.ReadAllText(Path.Combine(GetVersionPath(version), CatalogFileName))) ?? new();
            _snapshot = new CatalogSnapshot(version, items, items.ToDictionary(i => i.Id));
        }

        private record CatalogSnapshot(string? Version, List<MinecraftItem> Items, Dictionary<string, MinecraftItem> ById);
    }
}
