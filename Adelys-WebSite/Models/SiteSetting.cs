namespace Adelys_WebSite.Models
{
    /// <summary>
    /// Réglage du site modifiable par les admins, rangé sous une clé
    /// </summary>
    public class SiteSetting
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public static class SiteSettingKeys
    {
        public const string HeroImageUrl = "home.hero.image";
        public const string HeroImageAlt = "home.hero.alt";
    }
}
