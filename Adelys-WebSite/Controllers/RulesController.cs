using Adelys_WebSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Adelys_WebSite.Controllers
{
    public class RulesController : Controller
    {
        private readonly ILogger<RulesController> _logger;

        public RulesController(ILogger<RulesController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View("RulesIndex");
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