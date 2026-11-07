using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.SAL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adelys_WebSite.Controllers
{
    /// <summary>
    /// Images et aperçu des textes en markdown, communs au règlement et aux events
    /// </summary>
    [AutoValidateAntiforgeryToken]
    public class ContentController : Controller
    {
        private readonly IMarkdownBL _markdownBL;
        private readonly IContentImageStorage _imageStorage;

        public ContentController(IMarkdownBL markdownBL, IContentImageStorage imageStorage)
        {
            _markdownBL = markdownBL;
            _imageStorage = imageStorage;
        }

        // Rules/Image reste servi pour les comptes rendus écrits avant le passage à /Images
        [HttpGet("Images/{fileName}")]
        [HttpGet("Rules/Image/{fileName}")]
        public IActionResult Image(string fileName)
        {
            if (!_markdownBL.IsImageFileName(fileName))
            {
                return NotFound();
            }
            string path = _imageStorage.GetPath(fileName);
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            string contentType = Path.GetExtension(fileName) switch
            {
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };
            Response.Headers["X-Content-Type-Options"] = "nosniff";
            Response.Headers.CacheControl = "public, max-age=2592000, immutable";
            return PhysicalFile(path, contentType);
        }

        /// <summary>
        /// Rendu du markdown pour l'onglet Aperçu de l'éditeur, identique à celui des pages publiques
        /// </summary>
        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        public IActionResult Apercu(string? markdown)
        {
            return Content(_markdownBL.Render(markdown).ToString() ?? string.Empty, "text/html");
        }
    }
}
