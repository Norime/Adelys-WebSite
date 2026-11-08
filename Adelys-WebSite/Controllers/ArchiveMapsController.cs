using Microsoft.AspNetCore.Mvc;

namespace Adelys_WebSite.Controllers
{
    public class ArchiveMapsController : Controller
    {
        public IActionResult Index()
        {
            return View("ArchiveMapsIndex");
        }
    }
}