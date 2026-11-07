namespace Adelys_WebSite.Models
{
    /// <summary>
    /// Famille d'events, comme les jeux de la mort ou les events saisonniers, avec sa présentation
    /// </summary>
    public class EventCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        /// <summary>
        /// Texte court du bloc de la catégorie sur la page des events
        /// </summary>
        public string Summary { get; set; } = string.Empty;
        /// <summary>
        /// Présentation complète en markdown : principe, règles, déroulement
        /// </summary>
        public string? Description { get; set; }
        public string? CoverFileName { get; set; }
        /// <summary>
        /// Position du bloc sur la page des events, du plus petit au plus grand
        /// </summary>
        public int SortOrder { get; set; }
        public List<ServerEvent> Events { get; set; } = new();
    }

    public class ServerEvent
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public EventCategory Category { get; set; } = null!;
        public DateTime StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public string? Location { get; set; }
        /// <summary>
        /// Phrase d'accroche reprise sur les cartes et sur la page d'accueil
        /// </summary>
        public string Summary { get; set; } = string.Empty;
        /// <summary>
        /// Détails en markdown, les images pointent vers /Images/{fichier}
        /// </summary>
        public string? Content { get; set; }
        public string? CoverFileName { get; set; }
        /// <summary>
        /// Mis en avant sur la page d'accueil, un seul event à la fois
        /// </summary>
        public bool IsFeatured { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>
        /// Fin de l'event : sa date de fin, sinon la fin de sa journée de début
        /// </summary>
        public DateTime EndOrDayEnd => EndsAt ?? StartsAt.Date.AddDays(1);

        public bool IsOver(DateTime now) => EndOrDayEnd <= now;
    }

    /// <summary>
    /// Image envoyée depuis l'éditeur, rattachée à son event ou à sa catégorie à l'enregistrement du texte
    /// </summary>
    public class EventImage
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int? EventId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime UploadedAtUtc { get; set; }
    }
}
