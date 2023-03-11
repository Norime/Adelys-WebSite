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

        public ActionResult GetLawsPartialView()
        {
            return PartialView("~/Views/Rules/PartialViews/_laws.cshtml");
        }
        public ActionResult GetExceptionsPartialView()
        {
            return PartialView("~/Views/Rules/PartialViews/_exceptions.cshtml");
        }

        public ActionResult GetFactoriesPartialView()
        {
            return PartialView("~/Views/Rules/PartialViews/_factories.cshtml");
        }
        public ActionResult GetMonopoliesPartialView()
        {
            return PartialView("~/Views/Rules/PartialViews/_monopolies.cshtml");
        }
        
    }
}