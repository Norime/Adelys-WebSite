namespace Adelys_WebSite.Models
{
    public class Law
    {
        public int Id { get; set; }
        /// <summary>
        /// Numéro affiché, avec un suffixe éventuel comme 13bis
        /// </summary>
        public string Number { get; set; } = string.Empty;
        /// <summary>
        /// Texte de la loi en markdown
        /// </summary>
        public string Content { get; set; } = string.Empty;
        public List<LawAmendment> Amendments { get; set; } = new();
    }

    public class LawAmendment
    {
        public int Id { get; set; }
        public int LawId { get; set; }
        public DateTime AmendedOn { get; set; }
        public string Note { get; set; } = string.Empty;
    }

    public class ParliamentMeeting
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime HeldOn { get; set; }
        /// <summary>
        /// Compte rendu en markdown, les images pointent vers /Rules/Image/{fichier}
        /// </summary>
        public string Content { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }

    /// <summary>
    /// Image envoyée depuis l'éditeur, rattachée à sa réunion à l'enregistrement du compte rendu
    /// </summary>
    public class ParliamentImage
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int? MeetingId { get; set; }
        public DateTime UploadedAtUtc { get; set; }
    }
}
