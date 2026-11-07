using Adelys_WebSite.Models;
using System.ComponentModel.DataAnnotations;

namespace Adelys_WebSite.ViewModels
{
    public class GazetteLibraryViewModel
    {
        public List<Gazette> Gazettes { get; set; } = new();
        public bool CanManage { get; set; }
    }

    public class GazetteReaderViewModel
    {
        public Gazette Gazette { get; set; } = new();
        public bool CanManage { get; set; }
    }

    public class GazetteFormViewModel
    {
        public string? Slug { get; set; }

        public bool FromLibrary { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(150, ErrorMessage = "Le titre fait 150 caractères maximum")]
        [Display(Name = "Titre")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de parution est obligatoire")]
        [DataType(DataType.Date)]
        [Display(Name = "Date de parution")]
        public DateTime? PublishedOn { get; set; }

        [StringLength(500, ErrorMessage = "La description fait 500 caractères maximum")]
        [Display(Name = "Description (facultative)")]
        public string? Description { get; set; }

        [Display(Name = "Fichier PDF")]
        public IFormFile? Pdf { get; set; }

        public IFormFile? Cover { get; set; }
    }
}
