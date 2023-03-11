using Adelys_WebSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Adelys_WebSite.Controllers
{
    public class ArchiveMapsController : Controller
    {
        private readonly ILogger<ArchiveMapsController> _logger;

        public ArchiveMapsController(ILogger<ArchiveMapsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View("ArchiveMapsIndex");
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