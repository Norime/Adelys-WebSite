using Adelys_WebSite.SAL.Minecraft;

namespace Adelys_WebSite.SAL.Interface
{
    public interface IMinecraftItemCatalog
    {
        /// <summary>
        /// Version de Minecraft du catalogue chargé, null tant qu'aucun catalogue n'est prêt
        /// </summary>
        string? Version { get; }
        IReadOnlyList<MinecraftItem> Items { get; }
        MinecraftItem? Find(string? id);
        string? GetIconUrl(string? id);
    }
}
