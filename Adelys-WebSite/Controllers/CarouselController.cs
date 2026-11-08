using Adelys_WebSite.BL;
using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adelys_WebSite.Controllers
{
    /// <summary>
    /// Gestion des images du carrousel de la page d'accueil
    /// </summary>
    [AutoValidateAntiforgeryToken]
    [Authorize(Roles = SiteRoles.Admin)]
    [Route("Carrousel/[action]/{id?}")]
    public class CarouselController : Controller
    {
        private const long MaxRequestBytes = MarkdownBL.MaxImageBytes + 1024 * 1024;

        private readonly IHomeSlideBL _slideBL;
        private readonly IMarkdownBL _markdownBL;

        public CarouselController(IHomeSlideBL slideBL, IMarkdownBL markdownBL)
        {
            _slideBL = slideBL;
            _markdownBL = markdownBL;
        }

        [HttpGet("/Carrousel")]
        public IActionResult Index()
        {
            return View("CarouselIndex", new HomeSlidesViewModel { Slides = _slideBL.GetAllSlides() });
        }

        [HttpGet]
        public IActionResult Ajouter()
        {
            return View("CarouselForm", new HomeSlideFormViewModel());
        }

        [HttpPost]
        [RequestSizeLimit(MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
        public async Task<IActionResult> Ajouter(HomeSlideFormViewModel model)
        {
            if (model.Image == null)
            {
                ModelState.AddModelError(nameof(model.Image), "Choisis une image");
            }
            ValidateImage(model);
            if (!ModelState.IsValid)
            {
                return View("CarouselForm", model);
            }

            await _slideBL.CreateSlide(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Modifier(int id)
        {
            HomeSlide? slide = _slideBL.GetSlide(id);
            if (slide == null)
            {
                return NotFound();
            }
            return View("CarouselForm", new HomeSlideFormViewModel
            {
                Id = slide.Id,
                Title = slide.Title,
                Caption = slide.Caption,
                AltText = slide.AltText,
                CurrentImageUrl = slide.ImageUrl
            });
        }

        [HttpPost]
        [RequestSizeLimit(MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
        public async Task<IActionResult> Modifier(int id, HomeSlideFormViewModel model)
        {
            HomeSlide? slide = _slideBL.GetSlide(id);
            if (slide == null)
            {
                return NotFound();
            }

            model.Id = id;
            ValidateImage(model);
            if (!ModelState.IsValid)
            {
                model.CurrentImageUrl = slide.ImageUrl;
                return View("CarouselForm", model);
            }

            await _slideBL.UpdateSlide(slide, model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Supprimer(int id)
        {
            HomeSlide? slide = _slideBL.GetSlide(id);
            if (slide != null)
            {
                _slideBL.DeleteSlide(slide);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Deplacer(int id, int direction)
        {
            HomeSlide? slide = _slideBL.GetSlide(id);
            if (slide != null)
            {
                _slideBL.Move(slide, direction);
            }
            return RedirectToAction(nameof(Index));
        }

        private void ValidateImage(HomeSlideFormViewModel model)
        {
            if (model.Image == null)
            {
                return;
            }
            if (model.Image.Length > MarkdownBL.MaxImageBytes)
            {
                ModelState.AddModelError(nameof(model.Image), "L'image dépasse 10 Mo");
            }
            else if (_markdownBL.GetImageExtension(model.Image) == null)
            {
                ModelState.AddModelError(nameof(model.Image), "Seules les images JPEG, PNG, GIF et WebP sont acceptées");
            }
        }
    }
}
