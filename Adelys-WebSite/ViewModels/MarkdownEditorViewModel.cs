namespace Adelys_WebSite.ViewModels
{
    public class MarkdownEditorViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }
        public int MaxLength { get; set; }
        public int Rows { get; set; } = 10;
        /// <summary>
        /// Adresse d'envoi des images collées, déposées ou choisies avec le bouton, null pour un texte sans image
        /// </summary>
        public string? UploadUrl { get; set; }
    }
}
