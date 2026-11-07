using Adelys_WebSite.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Adelys_WebSite.ViewModels
{
    public class EventsIndexViewModel
    {
        /// <summary>
        /// Event à la une, sinon le prochain event ou celui en cours
        /// </summary>
        public ServerEvent? Current { get; set; }
        public List<EventCategoryBlockViewModel> Categories { get; set; } = new();
        public bool CanManage { get; set; }
    }

    public class EventCategoryBlockViewModel
    {
        public EventCategory Category { get; set; } = new();
        public ServerEvent? NextEvent { get; set; }
        public int EditionCount { get; set; }
    }

    public class EventCategoryPageViewModel
    {
        public EventCategory Category { get; set; } = new();
        public IHtmlContent Description { get; set; } = HtmlString.Empty;
        public List<ServerEvent> Upcoming { get; set; } = new();
        public List<ServerEvent> Past { get; set; } = new();
        public bool CanManage { get; set; }
    }

    public class EventCardViewModel
    {
        public ServerEvent Event { get; set; } = new();
        public bool IsOver { get; set; }
        public bool CanManage { get; set; }
    }

    public class EventPageViewModel
    {
        public ServerEvent Event { get; set; } = new();
        public IHtmlContent Html { get; set; } = HtmlString.Empty;
        public bool IsOver { get; set; }
        public bool CanManage { get; set; }
    }

    public class HomeViewModel
    {
        public ServerEvent? FeaturedEvent { get; set; }
    }

    public class EventFormViewModel : IValidatableObject
    {
        public string? Slug { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(150, ErrorMessage = "Le titre fait 150 caractères maximum")]
        [Display(Name = "Titre")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choisis une catégorie")]
        [Display(Name = "Catégorie")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "La date de début est obligatoire")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Début")]
        public DateTime? StartsAt { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Fin (facultative)")]
        public DateTime? EndsAt { get; set; }

        [StringLength(150, ErrorMessage = "Le lieu fait 150 caractères maximum")]
        [Display(Name = "Lieu (facultatif)")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "L'accroche est obligatoire")]
        [StringLength(300, ErrorMessage = "L'accroche fait 300 caractères maximum")]
        [Display(Name = "Accroche")]
        public string Summary { get; set; } = string.Empty;

        [StringLength(100000, ErrorMessage = "Le texte fait 100 000 caractères maximum")]
        [Display(Name = "Détails (facultatifs)")]
        public string? Content { get; set; }

        [Display(Name = "Image de couverture (facultative)")]
        public IFormFile? Cover { get; set; }

        [Display(Name = "Retirer l'image de couverture")]
        public bool RemoveCover { get; set; }

        [Display(Name = "Mettre en avant sur la page d'accueil")]
        public bool IsFeatured { get; set; }

        public string? CurrentCoverUrl { get; set; }
        public List<SelectListItem> Categories { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartsAt != null && EndsAt != null && EndsAt <= StartsAt)
            {
                yield return new ValidationResult("La fin doit être après le début", new[] { nameof(EndsAt) });
            }
        }
    }

    public class EventCategoryFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(80, ErrorMessage = "Le nom fait 80 caractères maximum")]
        [Display(Name = "Nom")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le texte du bloc est obligatoire")]
        [StringLength(500, ErrorMessage = "Le texte du bloc fait 500 caractères maximum")]
        [Display(Name = "Texte du bloc")]
        public string Summary { get; set; } = string.Empty;

        [StringLength(100000, ErrorMessage = "La présentation fait 100 000 caractères maximum")]
        [Display(Name = "Présentation complète")]
        public string? Description { get; set; }

        [Display(Name = "Image du bloc (facultative)")]
        public IFormFile? Cover { get; set; }

        [Display(Name = "Retirer l'image")]
        public bool RemoveCover { get; set; }

        [Range(0, 999, ErrorMessage = "Un nombre entre 0 et 999")]
        [Display(Name = "Position sur la page")]
        public int SortOrder { get; set; }

        public string? CurrentCoverUrl { get; set; }
    }

    /// <summary>
    /// Dates des events écrites en français, sans dépendre des cultures installées sur la machine
    /// </summary>
    public static class EventDates
    {
        private static readonly string[] Days = { "dimanche", "lundi", "mardi", "mercredi", "jeudi", "vendredi", "samedi" };
        private static readonly string[] Months = { "janvier", "février", "mars", "avril", "mai", "juin", "juillet", "août", "septembre", "octobre", "novembre", "décembre" };

        public static string Day(DateTime date) => $"{Days[(int)date.DayOfWeek]} {date.Day} {Months[date.Month - 1]} {date.Year}";

        public static string Time(DateTime date) => date.Minute == 0 ? $"{date.Hour}h" : $"{date.Hour}h{date.Minute:00}";

        public static string Format(ServerEvent serverEvent)
        {
            DateTime start = serverEvent.StartsAt;
            if (serverEvent.EndsAt is not DateTime end)
            {
                return $"Le {Day(start)} à {Time(start)}";
            }
            if (end.Date == start.Date)
            {
                return $"Le {Day(start)}, de {Time(start)} à {Time(end)}";
            }
            return $"Du {Day(start)} à {Time(start)} au {Day(end)} à {Time(end)}";
        }
    }
}
