namespace Adelys_WebSite.Models
{
    /// <summary>
    /// Image du carrousel « Un monde bâti par ses joueurs » sur la page d'accueil
    /// </summary>
    public class HomeSlide
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Caption { get; set; }
        /// <summary>
        /// Adresse de l'image : /Images/{fichier} pour une image envoyée, sinon une image livrée avec le site
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
