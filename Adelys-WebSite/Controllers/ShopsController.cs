using Adelys_WebSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Adelys_WebSite.Controllers
{
    public class ShopsController : Controller
    {
        private readonly ILogger<ShopsController> _logger;

        public ShopsController(ILogger<ShopsController> logger)
        {
            _logger = logger;
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
    }
}