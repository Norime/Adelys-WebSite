using Adelys_WebSite.BL.Interfaces;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Html;
using System.Text;
using System.Text.RegularExpressions;

namespace Adelys_WebSite.BL
{
    /// <summary>
    /// Rendu des textes écrits en markdown sur le site et contrôle des images qu'on y insère
    /// </summary>
    public partial class MarkdownBL : IMarkdownBL
    {
        public const long MaxImageBytes = 10L * 1024 * 1024;
        public const string ImageRequestPath = "/Images/";

        /// <summary>
        /// Images envoyées puis abandonnées sans enregistrer le texte, supprimées passé ce délai
        /// </summary>
        public static readonly TimeSpan OrphanImageDelay = TimeSpan.FromDays(1);

        private static readonly (string Extension, byte[] Signature)[] ImageSignatures =
        {
            (".jpg", new byte[] { 0xFF, 0xD8, 0xFF }),
            (".png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            (".gif", Encoding.ASCII.GetBytes("GIF8"))
        };

        // Le HTML brut est affiché comme du texte, seul le markdown met en forme
        private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
            .DisableHtml()
            .UseEmphasisExtras()
            .UsePipeTables()
            .UseAutoLinks()
            .UseSoftlineBreakAsHardlineBreak()
            .Build();

        public IHtmlContent Render(string? markdown)
        {
            MarkdownDocument document = Markdown.Parse(markdown ?? string.Empty, Pipeline);
            foreach (LinkInline link in document.Descendants<LinkInline>())
            {
                if (!IsSafeUrl(link.Url))
                {
                    link.Url = "#";
                }
            }
            return new HtmlString(document.ToHtml(Pipeline));
        }

        /// <summary>
        /// Extension correspondant au contenu réel du fichier, null si ce n'est pas une image acceptée
        /// </summary>
        public string? GetImageExtension(IFormFile file)
        {
            using Stream stream = file.OpenReadStream();
            byte[] header = new byte[12];
            int read = stream.Read(header, 0, header.Length);

            foreach ((string extension, byte[] signature) in ImageSignatures)
            {
                if (read >= signature.Length && header.AsSpan(0, signature.Length).SequenceEqual(signature))
                {
                    return extension;
                }
            }
            if (read == 12 && Encoding.ASCII.GetString(header, 0, 4) == "RIFF" && Encoding.ASCII.GetString(header, 8, 4) == "WEBP")
            {
                return ".webp";
            }
            return null;
        }

        public bool IsImageFileName(string fileName)
        {
            return ImageFileName().IsMatch(fileName);
        }

        /// <summary>
        /// Noms des images du site présentes dans un texte, y compris les anciennes adresses /Rules/Image/
        /// </summary>
        public HashSet<string> GetReferencedImages(string? markdown)
        {
            return ImageReference().Matches(markdown ?? string.Empty).Select(m => m.Groups[1].Value).ToHashSet();
        }

        private static bool IsSafeUrl(string? url)
        {
            return url == null
                || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith('/')
                || url.StartsWith('#');
        }

        [GeneratedRegex(@"^[0-9a-f]{32}\.(jpg|png|gif|webp)$")]
        private static partial Regex ImageFileName();

        [GeneratedRegex(@"/(?:Images|Rules/Image)/([0-9a-f]{32}\.(?:jpg|png|gif|webp))")]
        private static partial Regex ImageReference();
    }
}
