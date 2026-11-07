using Adelys_WebSite.BL;
using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Adelys_WebSite.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class EventsController : Controller
    {
        private const long MaxRequestBytes = MarkdownBL.MaxImageBytes + 1024 * 1024;

        private readonly IEventBL _eventBL;
        private readonly IMarkdownBL _markdownBL;

        private bool CanManage => User.IsInRole(SiteRoles.Admin);

        public EventsController(IEventBL eventBL, IMarkdownBL markdownBL)
        {
            _eventBL = eventBL;
            _markdownBL = markdownBL;
        }

        public IActionResult Index()
        {
            DateTime now = DateTime.Now;
            List<ServerEvent> events = _eventBL.GetAllEvents();

            return View("EventsIndex", new EventsIndexViewModel
            {
                Current = _eventBL.GetCurrentEvent(events, now),
                Categories = _eventBL.GetAllCategories().Select(c => new EventCategoryBlockViewModel
                {
                    Category = c,
                    NextEvent = events.FirstOrDefault(e => e.CategoryId == c.Id && !e.IsOver(now)),
                    EditionCount = events.Count(e => e.CategoryId == c.Id)
                }).ToList(),
                CanManage = CanManage
            });
        }

        [HttpGet("Events/Categorie/{slug}")]
        public IActionResult Categorie(string slug)
        {
            EventCategory? category = _eventBL.GetCategory(slug);
            if (category == null)
            {
                return NotFound();
            }

            DateTime now = DateTime.Now;
            return View("EventCategoryPage", new EventCategoryPageViewModel
            {
                Category = category,
                Description = _markdownBL.Render(category.Description),
                Upcoming = category.Events.Where(e => !e.IsOver(now)).OrderBy(e => e.StartsAt).ToList(),
                Past = category.Events.Where(e => e.IsOver(now)).OrderByDescending(e => e.StartsAt).ToList(),
                CanManage = CanManage
            });
        }

        [HttpGet("Events/Voir/{slug}")]
        public IActionResult Voir(string slug)
        {
            ServerEvent? serverEvent = _eventBL.GetEvent(slug);
            if (serverEvent == null)
            {
                return NotFound();
            }
            return View("EventPage", new EventPageViewModel
            {
                Event = serverEvent,
                Html = _markdownBL.Render(serverEvent.Content),
                IsOver = serverEvent.IsOver(DateTime.Now),
                CanManage = CanManage
            });
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpGet]
        public IActionResult Ajouter(int? categoryId)
        {
            return ShowEventForm(new EventFormViewModel { CategoryId = categoryId, StartsAt = DateTime.Today.AddDays(7).AddHours(21) });
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpPost]
        [RequestSizeLimit(MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
        public async Task<IActionResult> Ajouter(EventFormViewModel model)
        {
            ValidateEvent(model);
            if (!ModelState.IsValid)
            {
                return ShowEventForm(model);
            }

            ServerEvent serverEvent = await _eventBL.CreateEvent(model, User.Identity!.Name!);
            return RedirectToAction(nameof(Voir), new { slug = serverEvent.Slug });
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpGet("Events/Modifier/{slug}")]
        public IActionResult Modifier(string slug)
        {
            ServerEvent? serverEvent = _eventBL.GetEvent(slug);
            if (serverEvent == null)
            {
                return NotFound();
            }
            return ShowEventForm(new EventFormViewModel
            {
                Slug = serverEvent.Slug,
                Title = serverEvent.Title,
                CategoryId = serverEvent.CategoryId,
                StartsAt = serverEvent.StartsAt,
                EndsAt = serverEvent.EndsAt,
                Location = serverEvent.Location,
                Summary = serverEvent.Summary,
                Content = serverEvent.Content,
                IsFeatured = serverEvent.IsFeatured
            }, serverEvent);
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpPost("Events/Modifier/{slug}")]
        [RequestSizeLimit(MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
        public async Task<IActionResult> Modifier(string slug, EventFormViewModel model)
        {
            ServerEvent? serverEvent = _eventBL.GetEvent(slug);
            if (serverEvent == null)
            {
                return NotFound();
            }

            model.Slug = slug;
            ValidateEvent(model);
            if (!ModelState.IsValid)
            {
                return ShowEventForm(model, serverEvent);
            }

            await _eventBL.UpdateEvent(serverEvent, model);
            return RedirectToAction(nameof(Voir), new { slug });
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpPost("Events/Supprimer/{slug}")]
        public IActionResult Supprimer(string slug)
        {
            ServerEvent? serverEvent = _eventBL.GetEvent(slug);
            if (serverEvent == null)
            {
                return RedirectToAction(nameof(Index));
            }
            _eventBL.DeleteEvent(serverEvent);
            return RedirectToAction(nameof(Categorie), new { slug = serverEvent.Category.Slug });
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpGet]
        public IActionResult AjouterCategorie()
        {
            return View("EventCategoryForm", new EventCategoryFormViewModel { SortOrder = _eventBL.GetAllCategories().Select(c => c.SortOrder).DefaultIfEmpty(0).Max() + 1 });
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpPost]
        [RequestSizeLimit(MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
        public async Task<IActionResult> AjouterCategorie(EventCategoryFormViewModel model)
        {
            ValidateCategory(model);
            if (!ModelState.IsValid)
            {
                return View("EventCategoryForm", model);
            }

            EventCategory category = await _eventBL.CreateCategory(model);
            return RedirectToAction(nameof(Categorie), new { slug = category.Slug });
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpGet]
        public IActionResult ModifierCategorie(int id)
        {
            EventCategory? category = _eventBL.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }
            return View("EventCategoryForm", new EventCategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Summary = category.Summary,
                Description = category.Description,
                SortOrder = category.SortOrder,
                CurrentCoverUrl = CoverUrl(category.CoverFileName)
            });
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpPost]
        [RequestSizeLimit(MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
        public async Task<IActionResult> ModifierCategorie(int id, EventCategoryFormViewModel model)
        {
            EventCategory? category = _eventBL.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            model.Id = id;
            ValidateCategory(model);
            if (!ModelState.IsValid)
            {
                model.CurrentCoverUrl = CoverUrl(category.CoverFileName);
                return View("EventCategoryForm", model);
            }

            await _eventBL.UpdateCategory(category, model);
            return RedirectToAction(nameof(Categorie), new { slug = category.Slug });
        }

        [Authorize(Roles = SiteRoles.Admin)]
        [HttpPost]
        public IActionResult SupprimerCategorie(int id)
        {
            EventCategory? category = _eventBL.GetCategory(id);
            if (category != null && category.Events.Count == 0)
            {
                _eventBL.DeleteCategory(category);
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Image collée ou déposée dans l'éditeur, renvoie l'adresse à insérer dans le markdown
        /// </summary>
        [Authorize(Roles = SiteRoles.Admin)]
        [HttpPost]
        [RequestSizeLimit(MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
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

            return Json(new { url = await _eventBL.SaveImage(image, extension) });
        }

        private IActionResult ShowEventForm(EventFormViewModel model, ServerEvent? serverEvent = null)
        {
            model.Categories = _eventBL.GetAllCategories()
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList();
            model.CurrentCoverUrl = CoverUrl(serverEvent?.CoverFileName);
            return View("EventForm", model);
        }

        private void ValidateEvent(EventFormViewModel model)
        {
            if (model.CategoryId != null && _eventBL.GetCategory(model.CategoryId.Value) == null)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Cette catégorie n'existe plus");
            }

            ValidateCover(model.Cover, nameof(model.Cover));
        }

        private void ValidateCategory(EventCategoryFormViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Name) && _eventBL.IsCategoryNameTaken(model.Name, model.Id))
            {
                ModelState.AddModelError(nameof(model.Name), "Une catégorie porte déjà ce nom");
            }
            ValidateCover(model.Cover, nameof(model.Cover));
        }

        private void ValidateCover(IFormFile? cover, string field)
        {
            if (cover == null)
            {
                return;
            }
            if (cover.Length > MarkdownBL.MaxImageBytes)
            {
                ModelState.AddModelError(field, "L'image dépasse 10 Mo");
            }
            else if (_markdownBL.GetImageExtension(cover) == null)
            {
                ModelState.AddModelError(field, "Seules les images JPEG, PNG, GIF et WebP sont acceptées");
            }
        }

        private static string? CoverUrl(string? fileName)
        {
            return fileName != null ? MarkdownBL.ImageRequestPath + fileName : null;
        }
    }
}
