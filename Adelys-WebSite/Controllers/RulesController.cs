using Adelys_WebSite.BL;
using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adelys_WebSite.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class RulesController : Controller
    {
        private const string LawsSection = "lois";
        private const string MeetingsSection = "reunions";

        private readonly IRulesBL _rulesBL;
        private readonly IMarkdownBL _markdownBL;

        private bool CanManage => SiteRoles.CanEditContent(User);

        public RulesController(IRulesBL rulesBL, IMarkdownBL markdownBL)
        {
            _rulesBL = rulesBL;
            _markdownBL = markdownBL;
        }

        public IActionResult Index()
        {
            return View("RulesIndex");
        }

        public ActionResult GetLawsPartialView()
        {
            return PartialView("~/Views/Rules/PartialViews/_laws.cshtml", new LawsViewModel
            {
                Laws = _rulesBL.GetAllLaws().Select(l => new LawViewModel { Law = l, Html = _markdownBL.Render(l.Content) }).ToList(),
                CanManage = CanManage
            });
        }

        public ActionResult GetMeetingsPartialView()
        {
            return PartialView("~/Views/Rules/PartialViews/_meetings.cshtml", new MeetingsViewModel
            {
                Meetings = _rulesBL.GetAllMeetings().Select(m => new MeetingViewModel { Meeting = m, Html = _markdownBL.Render(m.Content) }).ToList(),
                CanManage = CanManage
            });
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpGet]
        public IActionResult AjouterLoi()
        {
            return View("LawForm", new LawFormViewModel());
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        public IActionResult AjouterLoi(LawFormViewModel model)
        {
            ValidateLaw(model);
            if (!ModelState.IsValid)
            {
                return View("LawForm", model);
            }

            _rulesBL.CreateLaw(model);
            return RedirectToAction(nameof(Index), null, LawsSection);
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpGet]
        public IActionResult ModifierLoi(int id)
        {
            Law? law = _rulesBL.GetLaw(id);
            if (law == null)
            {
                return NotFound();
            }
            return View("LawForm", new LawFormViewModel
            {
                Id = law.Id,
                Number = law.Number,
                Content = law.Content,
                Amendments = law.Amendments
                    .OrderBy(a => a.AmendedOn)
                    .Select(a => new LawAmendmentFormViewModel { AmendedOn = a.AmendedOn, Note = a.Note })
                    .ToList()
            });
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        public IActionResult ModifierLoi(int id, LawFormViewModel model)
        {
            Law? law = _rulesBL.GetLaw(id);
            if (law == null)
            {
                return NotFound();
            }

            model.Id = id;
            ValidateLaw(model);
            if (!ModelState.IsValid)
            {
                return View("LawForm", model);
            }

            _rulesBL.UpdateLaw(law, model);
            return RedirectToAction(nameof(Index), null, LawsSection);
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        public IActionResult SupprimerLoi(int id)
        {
            Law? law = _rulesBL.GetLaw(id);
            if (law != null)
            {
                _rulesBL.DeleteLaw(law);
            }
            return RedirectToAction(nameof(Index), null, LawsSection);
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpGet]
        public IActionResult AjouterReunion()
        {
            return View("MeetingForm", new MeetingFormViewModel { HeldOn = DateTime.Today });
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        public IActionResult AjouterReunion(MeetingFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("MeetingForm", model);
            }

            _rulesBL.CreateMeeting(model, User.Identity!.Name!);
            return RedirectToAction(nameof(Index), null, MeetingsSection);
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpGet]
        public IActionResult ModifierReunion(int id)
        {
            ParliamentMeeting? meeting = _rulesBL.GetMeeting(id);
            if (meeting == null)
            {
                return NotFound();
            }
            return View("MeetingForm", new MeetingFormViewModel
            {
                Id = meeting.Id,
                Title = meeting.Title,
                HeldOn = meeting.HeldOn,
                Content = meeting.Content
            });
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        public IActionResult ModifierReunion(int id, MeetingFormViewModel model)
        {
            ParliamentMeeting? meeting = _rulesBL.GetMeeting(id);
            if (meeting == null)
            {
                return NotFound();
            }

            model.Id = id;
            if (!ModelState.IsValid)
            {
                return View("MeetingForm", model);
            }

            _rulesBL.UpdateMeeting(meeting, model);
            return RedirectToAction(nameof(Index), null, MeetingsSection);
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        public IActionResult SupprimerReunion(int id)
        {
            ParliamentMeeting? meeting = _rulesBL.GetMeeting(id);
            if (meeting != null)
            {
                _rulesBL.DeleteMeeting(meeting);
            }
            return RedirectToAction(nameof(Index), null, MeetingsSection);
        }

        /// <summary>
        /// Image collée ou déposée dans l'éditeur, renvoie l'adresse à insérer dans le markdown
        /// </summary>
        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        [RequestSizeLimit(MarkdownBL.MaxImageBytes + 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = MarkdownBL.MaxImageBytes + 1024 * 1024)]
        public async Task<IActionResult> EnvoyerImage(IFormFile? image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest(new { error = "Aucune image reçue" });
            }
            if (image.Length > MarkdownBL.MaxImageBytes)
            {
                return BadRequest(new { error = "L'image dépasse 10 Mo" });
            }
            string? extension = _markdownBL.GetImageExtension(image);
            if (extension == null)
            {
                return BadRequest(new { error = "Seules les images JPEG, PNG, GIF et WebP sont acceptées" });
            }

            return Json(new { url = await _rulesBL.SaveImage(image, extension) });
        }

        private void ValidateLaw(LawFormViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Number) && _rulesBL.IsLawNumberTaken(model.Number.Trim(), model.Id))
            {
                ModelState.AddModelError(nameof(model.Number), $"La loi n°{model.Number.Trim()} existe déjà");
            }
        }
    }
}
