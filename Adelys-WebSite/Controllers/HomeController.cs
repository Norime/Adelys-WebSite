using Adelys_WebSite.BL;
using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Adelys_WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventBL _eventBL;
        private readonly IHomeSlideBL _slideBL;
        private readonly IHomeHeroBL _heroBL;
        private readonly IMarkdownBL _markdownBL;

        public HomeController(ILogger<HomeController> logger, IEventBL eventBL, IHomeSlideBL slideBL, IHomeHeroBL heroBL, IMarkdownBL markdownBL)
        {
            _logger = logger;
            _eventBL = eventBL;
            _slideBL = slideBL;
            _heroBL = heroBL;
            _markdownBL = markdownBL;
        }

        public IActionResult Index()
        {
            HomeViewModel model = new HomeViewModel { CanManage = User.IsInRole(SiteRoles.Admin) };
            try
            {
                model.FeaturedEvent = _eventBL.GetFeaturedEvent(DateTime.Now);
                model.Slides = _slideBL.GetAllSlides();
                model.Hero = _heroBL.GetHero();
            }
            catch (Exception ex)
            {
                // L'accueil reste affiché sans l'event ni le carrousel si la base du site ne répond pas
                _logger.LogError(ex, "Impossible de charger l'event mis en avant et le carrousel");
            }
            return View("HomeIndex", model);
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpGet("Accueil/ImageDeFond")]
        public IActionResult ImageDeFond()
        {
            HomeHero hero = _heroBL.GetHero();
            return View("HomeHeroForm", new HomeHeroFormViewModel { AltText = hero.AltText, Current = hero });
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpPost("Accueil/ImageDeFond")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(MarkdownBL.MaxImageBytes + 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = MarkdownBL.MaxImageBytes + 1024 * 1024)]
        public async Task<IActionResult> ImageDeFond(HomeHeroFormViewModel model)
        {
            if (model.Image != null && model.Image.Length > MarkdownBL.MaxImageBytes)
            {
                ModelState.AddModelError(nameof(model.Image), "L'image dépasse 10 Mo");
            }
            else if (model.Image != null && _markdownBL.GetImageExtension(model.Image) == null)
            {
                ModelState.AddModelError(nameof(model.Image), "Seules les images JPEG, PNG, GIF et WebP sont acceptées");
            }
            if (!ModelState.IsValid)
            {
                model.Current = _heroBL.GetHero();
                return View("HomeHeroForm", model);
            }

            await _heroBL.UpdateHero(model);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpPost("Accueil/ImageDeFond/ParDefaut")]
        [ValidateAntiForgeryToken]
        public IActionResult ImageDeFondParDefaut()
        {
            _heroBL.ResetHero();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("mentions-legales")]
        public IActionResult MentionsLegales()
        {
            return View("LegalNotice");
        }

        [HttpGet("confidentialite")]
        public IActionResult Confidentialite()
        {
            return View("PrivacyPolicy");
        }

        public IActionResult Privacy()
        {
            return RedirectToActionPermanent(nameof(Confidentialite));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}