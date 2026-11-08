namespace Adelys_WebSite.Models
{
    public class Gazette
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime PublishedOn { get; set; }
        public string PdfFileName { get; set; } = string.Empty;
        public long PdfSizeBytes { get; set; }
        public string? CoverFileName { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
