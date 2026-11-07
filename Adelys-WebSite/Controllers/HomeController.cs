using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Adelys_WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventBL _eventBL;

        public HomeController(ILogger<HomeController> logger, IEventBL eventBL)
        {
            _logger = logger;
            _eventBL = eventBL;
        }

        public IActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();
            try
            {
                model.FeaturedEvent = _eventBL.GetFeaturedEvent(DateTime.Now);
            }
            catch (Exception ex)
            {
                // L'accueil reste affiché sans l'event si la base du site ne répond pas
                _logger.LogError(ex, "Impossible de charger l'event mis en avant");
            }
            return View("HomeIndex", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}