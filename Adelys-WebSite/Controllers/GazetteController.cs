using Adelys_WebSite.BL;
using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adelys_WebSite.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class GazetteController : Controller
    {
        private readonly IGazetteBL _gazetteBL;

        private bool CanManage => User.IsInRole(SiteRoles.Admin) || User.IsInRole(SiteRoles.Redacteur);

        public GazetteController(IGazetteBL gazetteBL)
        {
            _gazetteBL = gazetteBL;
        }

        public IActionResult Index()
        {
            return View("GazetteIndex", new GazetteLibraryViewModel
            {
                Gazettes = _gazetteBL.GetAllGazettes(),
                CanManage = CanManage
            });
        }

        [HttpGet("Gazette/Lire/{slug}")]
        public IActionResult Lire(string slug)
        {
            Gazette? gazette = _gazetteBL.GetGazette(slug);
            if (gazette == null)
            {
                return NotFound();
            }
            return View("GazetteRead", new GazetteReaderViewModel { Gazette = gazette, CanManage = CanManage });
        }

        [HttpGet("Gazette/Fichier/{slug}")]
        public IActionResult Fichier(string slug, bool download = false)
        {
            Gazette? gazette = _gazetteBL.GetGazette(slug);
            if (gazette == null)
            {
                return NotFound();
            }

            string path = _gazetteBL.GetFilePath(gazette.PdfFileName);
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }
            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return PhysicalFile(path, "application/pdf", download ? $"{gazette.Slug}.pdf" : null, enableRangeProcessing: true);
        }

        [HttpGet("Gazette/Couverture/{slug}")]
        public IActionResult Couverture(string slug)
        {
            Gazette? gazette = _gazetteBL.GetGazette(slug);
            if (gazette?.CoverFileName == null)
            {
                return NotFound();
            }

            string path = _gazetteBL.GetFilePath(gazette.CoverFileName);
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }
            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return PhysicalFile(path, "image/jpeg");
        }

        [Authorize(Roles = SiteRoles.GazetteEditors)]
        [HttpGet]
        public IActionResult Deposer()
        {
            return View("GazetteForm", new GazetteFormViewModel { PublishedOn = DateTime.Today });
        }

        [Authorize(Roles = SiteRoles.GazetteEditors)]
        [HttpPost]
        [RequestSizeLimit(GazetteBL.MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = GazetteBL.MaxRequestBytes)]
        public async Task<IActionResult> Deposer(GazetteFormViewModel model)
        {
            if (model.Pdf == null)
            {
                ModelState.AddModelError(nameof(model.Pdf), "Choisis le fichier PDF de la gazette");
            }
            ValidateFiles(model);
            if (!ModelState.IsValid)
            {
                return View("GazetteForm", model);
            }

            Gazette gazette = await _gazetteBL.CreateGazette(model, User.Identity!.Name!);
            return RedirectToAction(nameof(Lire), new { slug = gazette.Slug });
        }

        [Authorize(Roles = SiteRoles.GazetteEditors)]
        [HttpGet("Gazette/Modifier/{slug}")]
        public IActionResult Modifier(string slug, bool fromLibrary = false)
        {
            Gazette? gazette = _gazetteBL.GetGazette(slug);
            if (gazette == null)
            {
                return NotFound();
            }
            return View("GazetteForm", new GazetteFormViewModel
            {
                Slug = gazette.Slug,
                FromLibrary = fromLibrary,
                Title = gazette.Title,
                PublishedOn = gazette.PublishedOn,
                Description = gazette.Description
            });
        }

        [Authorize(Roles = SiteRoles.GazetteEditors)]
        [HttpPost("Gazette/Modifier/{slug}")]
        [RequestSizeLimit(GazetteBL.MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = GazetteBL.MaxRequestBytes)]
        public async Task<IActionResult> Modifier(string slug, GazetteFormViewModel model)
        {
            Gazette? gazette = _gazetteBL.GetGazette(slug);
            if (gazette == null)
            {
                return NotFound();
            }

            model.Slug = slug;
            ValidateFiles(model);
            if (!ModelState.IsValid)
            {
                return View("GazetteForm", model);
            }

            await _gazetteBL.UpdateGazette(gazette, model);
            return RedirectToAction(nameof(Lire), new { slug });
        }

        [Authorize(Roles = SiteRoles.GazetteEditors)]
        [HttpPost("Gazette/Supprimer/{slug}")]
        public IActionResult Supprimer(string slug)
        {
            Gazette? gazette = _gazetteBL.GetGazette(slug);
            if (gazette != null)
            {
                _gazetteBL.DeleteGazette(gazette);
            }
            return RedirectToAction(nameof(Index));
        }

        private void ValidateFiles(GazetteFormViewModel model)
        {
            if (model.Pdf != null && model.Pdf.Length > GazetteBL.MaxPdfBytes)
            {
                ModelState.AddModelError(nameof(model.Pdf), "Le PDF dépasse 100 Mo");
            }
            else if (model.Pdf != null && !_gazetteBL.IsPdf(model.Pdf))
            {
                ModelState.AddModelError(nameof(model.Pdf), "Ce fichier n'est pas un PDF");
            }

            // La couverture est facultative : une image invalide est ignorée sans bloquer le dépôt
            if (model.Cover != null && (model.Cover.Length > GazetteBL.MaxCoverBytes || !_gazetteBL.IsJpeg(model.Cover)))
            {
                model.Cover = null;
            }
        }
    }
}
