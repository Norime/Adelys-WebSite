namespace Adelys_WebSite.Models
{
    /// <summary>
    /// Informations des mentions légales, lues dans la section Legal de la configuration
    /// </summary>
    public class LegalInfo
    {
        /// <summary>
        /// Nom de la personne qui édite le site, laissé vide pour rester anonyme si l'hébergeur connaît son identité
        /// </summary>
        public string EditorName { get; set; } = string.Empty;
        public string PublicationDirector { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string HostName { get; set; } = string.Empty;
        public string HostAddress { get; set; } = string.Empty;
        public string HostPhone { get; set; } = string.Empty;
        /// <summary>
        /// Date de la dernière mise à jour des pages légales, au format JJ/MM/AAAA
        /// </summary>
        public string LastUpdated { get; set; } = string.Empty;
    }
}
