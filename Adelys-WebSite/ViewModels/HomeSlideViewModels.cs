using Adelys_WebSite.Models;
using System.ComponentModel.DataAnnotations;

namespace Adelys_WebSite.ViewModels
{
    public class HomeSlidesViewModel
    {
        public List<HomeSlide> Slides { get; set; } = new();
    }

    public class HomeSlideFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(100, ErrorMessage = "Le titre fait 100 caractères maximum")]
        [Display(Name = "Titre")]
        public string Title { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "La légende fait 300 caractères maximum")]
        [Display(Name = "Légende (facultative)")]
        public string? Caption { get; set; }

        [Required(ErrorMessage = "Décris l'image pour les personnes qui ne la voient pas")]
        [StringLength(200, ErrorMessage = "La description fait 200 caractères maximum")]
        [Display(Name = "Description de l'image")]
        public string AltText { get; set; } = string.Empty;

        [Display(Name = "Image")]
        public IFormFile? Image { get; set; }

        public string? CurrentImageUrl { get; set; }
    }

    public class HomeHero
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    public class HomeHeroFormViewModel
    {
        [Display(Name = "Nouvelle image")]
        public IFormFile? Image { get; set; }

        [Required(ErrorMessage = "Décris l'image pour les personnes qui ne la voient pas")]
        [StringLength(200, ErrorMessage = "La description fait 200 caractères maximum")]
        [Display(Name = "Description de l'image")]
        public string AltText { get; set; } = string.Empty;

        public HomeHero Current { get; set; } = new();
    }
}
