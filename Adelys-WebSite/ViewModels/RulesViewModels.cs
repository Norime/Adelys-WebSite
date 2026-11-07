using Adelys_WebSite.Models;
using Microsoft.AspNetCore.Html;
using System.ComponentModel.DataAnnotations;

namespace Adelys_WebSite.ViewModels
{
    public class LawViewModel
    {
        public Law Law { get; set; } = new();
        public IHtmlContent Html { get; set; } = HtmlString.Empty;
    }

    public class LawsViewModel
    {
        public List<LawViewModel> Laws { get; set; } = new();
        public bool CanManage { get; set; }
    }

    public class MeetingViewModel
    {
        public ParliamentMeeting Meeting { get; set; } = new();
        public IHtmlContent Html { get; set; } = HtmlString.Empty;
    }

    public class MeetingsViewModel
    {
        public List<MeetingViewModel> Meetings { get; set; } = new();
        public bool CanManage { get; set; }
    }

    public class LawFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Le numéro est obligatoire")]
        [RegularExpression(@"^\d{1,4}[a-z]{0,6}$", ErrorMessage = "Un numéro suivi éventuellement d'un suffixe en minuscules, par exemple 13bis")]
        [Display(Name = "Numéro")]
        public string Number { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le texte de la loi est obligatoire")]
        [StringLength(5000, ErrorMessage = "Le texte fait 5000 caractères maximum")]
        [Display(Name = "Texte de la loi")]
        public string Content { get; set; } = string.Empty;

        public List<LawAmendmentFormViewModel> Amendments { get; set; } = new();
    }

    public class LawAmendmentFormViewModel
    {
        [Required(ErrorMessage = "La date de la modification est obligatoire")]
        [DataType(DataType.Date)]
        public DateTime? AmendedOn { get; set; }

        [Required(ErrorMessage = "Décris la modification")]
        [StringLength(300, ErrorMessage = "La description fait 300 caractères maximum")]
        public string Note { get; set; } = string.Empty;
    }

    public class LawAmendmentRowViewModel
    {
        /// <summary>
        /// Index de la ligne dans le formulaire, null pour le modèle copié en JavaScript
        /// </summary>
        public int? Index { get; set; }
        public LawAmendmentFormViewModel Amendment { get; set; } = new();
    }

    public class MeetingFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(150, ErrorMessage = "Le titre fait 150 caractères maximum")]
        [Display(Name = "Titre")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de la réunion est obligatoire")]
        [DataType(DataType.Date)]
        [Display(Name = "Date de la réunion")]
        public DateTime? HeldOn { get; set; }

        [Required(ErrorMessage = "Le compte rendu est obligatoire")]
        [StringLength(100000, ErrorMessage = "Le compte rendu fait 100 000 caractères maximum")]
        [Display(Name = "Compte rendu")]
        public string Content { get; set; } = string.Empty;
    }
}
