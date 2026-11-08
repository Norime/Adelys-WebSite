using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Adelys_WebSite.Controllers
{
    [Authorize(Roles = SiteRoles.Admin)]
    [AutoValidateAntiforgeryToken]
    public class UsersController : Controller
    {
        private readonly ISiteUserBL _siteUserBL;

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public UsersController(ISiteUserBL siteUserBL)
        {
            _siteUserBL = siteUserBL;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("UsersIndex", BuildViewModel());
        }

        [HttpPost]
        public IActionResult Create([Bind(Prefix = "NewUser")] CreateUserViewModel model)
        {
            if (!SiteRoles.All.Contains(model.Role))
            {
                ModelState.AddModelError(string.Empty, "Rôle inconnu");
            }
            if (ModelState.IsValid && _siteUserBL.GetUser(model.Username) != null)
            {
                ModelState.AddModelError(string.Empty, "Cet identifiant existe déjà");
            }
            if (!ModelState.IsValid)
            {
                UsersViewModel invalidViewModel = BuildViewModel();
                invalidViewModel.NewUser = model;
                return View("UsersIndex", invalidViewModel);
            }

            string password = _siteUserBL.CreateUser(model.Username, model.Role);
            return ShowGeneratedPassword(model.Username, password);
        }

        [HttpPost]
        public IActionResult ResetPassword(int id)
        {
            SiteUser? user = GetOtherUser(id);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return ShowGeneratedPassword(user.Username, _siteUserBL.ResetPassword(user));
        }

        [HttpPost]
        public IActionResult SetActive(int id, bool isActive)
        {
            SiteUser? user = GetOtherUser(id);
            if (user != null)
            {
                _siteUserBL.SetActive(user, isActive);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            SiteUser? user = GetOtherUser(id);
            if (user != null)
            {
                _siteUserBL.DeleteUser(user);
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Un administrateur ne peut pas agir sur son propre compte, il reste donc toujours un administrateur actif
        /// </summary>
        private SiteUser? GetOtherUser(int id)
        {
            return id == CurrentUserId ? null : _siteUserBL.GetUser(id);
        }

        private IActionResult ShowGeneratedPassword(string username, string password)
        {
            // Affiché une seule fois, sans redirection pour ne pas le faire transiter ailleurs
            ModelState.Clear();
            UsersViewModel viewModel = BuildViewModel();
            viewModel.GeneratedForUsername = username;
            viewModel.GeneratedPassword = password;
            return View("UsersIndex", viewModel);
        }

        private UsersViewModel BuildViewModel()
        {
            return new UsersViewModel
            {
                Users = _siteUserBL.GetAllUsers(),
                CurrentUserId = CurrentUserId
            };
        }
    }
}
