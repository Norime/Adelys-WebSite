using Adelys_WebSite.BL;
using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Adelys_WebSite.Controllers
{
    public class ShopsController : Controller
    {
        private DataStorageBL _dataStorageBL;
        private readonly ILogger<ShopsController> _logger;

        public ShopsController(ILogger<ShopsController> logger)
        {
            _logger = logger;
            _dataStorageBL = new();
        }

        public IActionResult Index()
        {
            return View("ShopsIndex");
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

        public ActionResult GetShopsFromCityPartialView(string cityName)
        {
            _dataStorageBL.GetListShops();
            ShopsViewModel viewModel = new()
            {
                dataStorageBL = _dataStorageBL
            };
            return PartialView("~/Views/Shops/PartialViews/_displayListeShops.cshtml", viewModel);
        }
    }
}