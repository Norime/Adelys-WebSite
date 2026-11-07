using Adelys_WebSite.BL;
using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Adelys_WebSite.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private readonly ISiteUserBL _siteUserBL;

        public AccountController(ISiteUserBL siteUserBL)
        {
            _siteUserBL = siteUserBL;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("AccountLogin", new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AccountLogin", model);
            }

            SiteUser? user = _siteUserBL.Authenticate(model.Username, model.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Identifiant ou mot de passe incorrect, ou compte bloqué temporairement");
                return View("AccountLogin", model);
            }

            await SignIn(user);
            if (user.MustChangePassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }
            if (Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View("AccountChangePassword", new ChangePasswordViewModel());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AccountChangePassword", model);
            }

            SiteUser? user = _siteUserBL.GetUser(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
            if (user == null || !_siteUserBL.ChangePassword(user, model.CurrentPassword, model.NewPassword))
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "Le mot de passe actuel est incorrect");
                return View("AccountChangePassword", model);
            }

            await SignIn(user);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View("AccountAccessDenied");
        }

        private async Task SignIn(SiteUser user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(SiteUserBL.SecurityStampClaim, user.SecurityStamp)
            };
            if (user.MustChangePassword)
            {
                claims.Add(new Claim(SiteUserBL.MustChangePasswordClaim, "true"));
            }

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
    }
}
