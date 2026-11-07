using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.SAL.Interface;
using Adelys_WebSite.ViewModels;

namespace Adelys_WebSite.BL
{
    public class EventBL : IEventBL
    {
        private readonly IEventRepository _repository;
        private readonly IContentImageStorage _imageStorage;
        private readonly IMarkdownBL _markdownBL;

        public EventBL(IEventRepository repository, IContentImageStorage imageStorage, IMarkdownBL markdownBL)
        {
            _repository = repository;
            _imageStorage = imageStorage;
            _markdownBL = markdownBL;
        }

        public List<EventCategory> GetAllCategories()
        {
            return _repository.GetAllCategories();
        }

        public EventCategory? GetCategory(int id)
        {
            return _repository.GetCategory(id);
        }

        public bool IsCategoryNameTaken(string name, int? exceptId)
        {
            return _repository.IsCategoryNameTaken(name.Trim(), exceptId);
        }

        public EventCategory? GetCategory(string slug)
        {
            return _repository.GetCategoryBySlug(slug);
        }

        public async Task<EventCategory> CreateCategory(EventCategoryFormViewModel form)
        {
            string name = form.Name.Trim();
            EventCategory category = new EventCategory
            {
                // a-venir reste libre pour une éventuelle ancre de la page des events
                Slug = SlugHelper.MakeUnique(SlugHelper.Slugify(name, "events"), slug => slug == "a-venir" || _repository.GetCategoryBySlug(slug) != null)
            };
            Fill(category, form);
            if (form.Cover != null)
            {
                category.CoverFileName = await SaveCover(form.Cover);
            }

            try
            {
                _repository.CreateCategory(category);
            }
            catch
            {
                _imageStorage.Delete(category.CoverFileName);
                throw;
            }
            SyncImages(category.Description, _repository.GetCategoryImages(category.Id), image => image.CategoryId = category.Id);
            return category;
        }

        /// <summary>
        /// L'adresse de la catégorie ne change pas avec son nom pour ne pas casser les liens partagés
        /// </summary>
        public async Task UpdateCategory(EventCategory category, EventCategoryFormViewModel form)
        {
            string? oldCover = category.CoverFileName;
            string? newCover = form.Cover != null ? await SaveCover(form.Cover) : null;

            try
            {
                Fill(category, form);
                if (newCover != null)
                {
                    category.CoverFileName = newCover;
                }
                else if (form.RemoveCover)
                {
                    category.CoverFileName = null;
                }
                _repository.Update();
            }
            catch
            {
                _imageStorage.Delete(newCover);
                throw;
            }

            if (category.CoverFileName != oldCover)
            {
                _imageStorage.Delete(oldCover);
            }
            SyncImages(category.Description, _repository.GetCategoryImages(category.Id), image => image.CategoryId = category.Id);
        }

        public void DeleteCategory(EventCategory category)
        {
            List<EventImage> images = _repository.GetCategoryImages(category.Id);
            _repository.DeleteCategory(category);
            DeleteImages(images);
            _imageStorage.Delete(category.CoverFileName);
        }

        public List<ServerEvent> GetAllEvents()
        {
            return _repository.GetAllEvents();
        }

        public ServerEvent? GetEvent(string slug)
        {
            return _repository.GetEventBySlug(slug);
        }

        /// <summary>
        /// Event à afficher sur l'accueil, il disparaît de lui-même une fois terminé
        /// </summary>
        public ServerEvent? GetFeaturedEvent(DateTime now)
        {
            ServerEvent? featured = _repository.GetFeaturedEvent();
            return featured != null && !featured.IsOver(now) ? featured : null;
        }

        /// <summary>
        /// Event du moment : celui à la une, sinon l'event en cours ou le prochain
        /// </summary>
        public ServerEvent? GetCurrentEvent(List<ServerEvent> events, DateTime now)
        {
            return events.FirstOrDefault(e => e.IsFeatured && !e.IsOver(now))
                ?? events.Where(e => !e.IsOver(now)).OrderBy(e => e.StartsAt).FirstOrDefault();
        }

        public async Task<ServerEvent> CreateEvent(EventFormViewModel form, string createdBy)
        {
            ServerEvent serverEvent = new ServerEvent
            {
                Slug = SlugHelper.MakeUnique(SlugHelper.Slugify(form.Title, "event"), slug => _repository.GetEventBySlug(slug) != null),
                CreatedBy = createdBy,
                CreatedAtUtc = DateTime.UtcNow
            };
            Fill(serverEvent, form);
            if (form.Cover != null)
            {
                serverEvent.CoverFileName = await SaveCover(form.Cover);
            }

            try
            {
                UnfeatureOthers(serverEvent);
                _repository.CreateEvent(serverEvent);
            }
            catch
            {
                _imageStorage.Delete(serverEvent.CoverFileName);
                throw;
            }
            SyncImages(serverEvent.Content, _repository.GetEventImages(serverEvent.Id), image => image.EventId = serverEvent.Id);
            return serverEvent;
        }

        /// <summary>
        /// L'adresse de l'event ne change pas avec son titre pour ne pas casser les liens partagés
        /// </summary>
        public async Task UpdateEvent(ServerEvent serverEvent, EventFormViewModel form)
        {
            string? oldCover = serverEvent.CoverFileName;
            string? newCover = form.Cover != null ? await SaveCover(form.Cover) : null;

            try
            {
                Fill(serverEvent, form);
                serverEvent.UpdatedAtUtc = DateTime.UtcNow;
                if (newCover != null)
                {
                    serverEvent.CoverFileName = newCover;
                }
                else if (form.RemoveCover)
                {
                    serverEvent.CoverFileName = null;
                }
                UnfeatureOthers(serverEvent);
                _repository.Update();
            }
            catch
            {
                _imageStorage.Delete(newCover);
                throw;
            }

            if (serverEvent.CoverFileName != oldCover)
            {
                _imageStorage.Delete(oldCover);
            }
            SyncImages(serverEvent.Content, _repository.GetEventImages(serverEvent.Id), image => image.EventId = serverEvent.Id);
        }

        public void DeleteEvent(ServerEvent serverEvent)
        {
            List<EventImage> images = _repository.GetEventImages(serverEvent.Id);
            _repository.DeleteEvent(serverEvent);
            DeleteImages(images);
            _imageStorage.Delete(serverEvent.CoverFileName);
        }

        public async Task<string> SaveImage(IFormFile file, string extension)
        {
            DeleteImages(_repository.GetOrphanImages(DateTime.UtcNow - MarkdownBL.OrphanImageDelay));

            string fileName = await _imageStorage.Save(file, extension);
            try
            {
                _repository.CreateImage(new EventImage { FileName = fileName, UploadedAtUtc = DateTime.UtcNow });
            }
            catch
            {
                _imageStorage.Delete(fileName);
                throw;
            }
            return MarkdownBL.ImageRequestPath + fileName;
        }

        private Task<string> SaveCover(IFormFile cover)
        {
            return _imageStorage.Save(cover, _markdownBL.GetImageExtension(cover)!);
        }

        private static void Fill(EventCategory category, EventCategoryFormViewModel form)
        {
            category.Name = form.Name.Trim();
            category.Summary = form.Summary.Trim();
            category.Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description;
            category.SortOrder = form.SortOrder;
        }

        private static void Fill(ServerEvent serverEvent, EventFormViewModel form)
        {
            serverEvent.Title = form.Title.Trim();
            serverEvent.CategoryId = form.CategoryId!.Value;
            serverEvent.StartsAt = form.StartsAt!.Value;
            serverEvent.EndsAt = form.EndsAt;
            serverEvent.Location = string.IsNullOrWhiteSpace(form.Location) ? null : form.Location.Trim();
            serverEvent.Summary = form.Summary.Trim();
            serverEvent.Content = string.IsNullOrWhiteSpace(form.Content) ? null : form.Content;
            serverEvent.IsFeatured = form.IsFeatured;
        }

        /// <summary>
        /// Un seul event à la une : en mettre un en avant retire les autres
        /// </summary>
        private void UnfeatureOthers(ServerEvent serverEvent)
        {
            if (!serverEvent.IsFeatured)
            {
                return;
            }
            foreach (ServerEvent other in _repository.GetFeaturedEvents().Where(e => e.Id != serverEvent.Id))
            {
                other.IsFeatured = false;
            }
        }

        /// <summary>
        /// Rattache au texte les images qu'il contient et supprime celles qui en ont été retirées
        /// </summary>
        private void SyncImages(string? content, List<EventImage> current, Action<EventImage> attach)
        {
            HashSet<string> referenced = _markdownBL.GetReferencedImages(content);

            List<EventImage> removed = current.Where(i => !referenced.Contains(i.FileName)).ToList();
            foreach (EventImage image in _repository.GetImages(referenced).Where(i => i.EventId == null && i.CategoryId == null))
            {
                attach(image);
            }
            _repository.Update();
            DeleteImages(removed);
        }

        private void DeleteImages(List<EventImage> images)
        {
            if (images.Count == 0)
            {
                return;
            }
            _repository.DeleteImages(images);
            foreach (EventImage image in images)
            {
                _imageStorage.Delete(image.FileName);
            }
        }
    }
}
