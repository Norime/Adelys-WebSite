using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;
using Microsoft.EntityFrameworkCore;

namespace Adelys_WebSite.DAL.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly SiteDbContext _context;

        public EventRepository(SiteDbContext context)
        {
            _context = context;
        }

        public List<EventCategory> GetAllCategories()
        {
            return _context.EventCategories.OrderBy(c => c.SortOrder).ThenBy(c => c.Id).ToList();
        }

        public EventCategory? GetCategory(int id)
        {
            return _context.EventCategories.Include(c => c.Events).FirstOrDefault(c => c.Id == id);
        }

        public EventCategory? GetCategoryBySlug(string slug)
        {
            return _context.EventCategories.Include(c => c.Events).FirstOrDefault(c => c.Slug == slug);
        }

        public bool IsCategoryNameTaken(string name, int? exceptId)
        {
            return _context.EventCategories.Any(c => c.Name == name && c.Id != exceptId);
        }

        public void CreateCategory(EventCategory category)
        {
            _context.EventCategories.Add(category);
            _context.SaveChanges();
        }

        public void DeleteCategory(EventCategory category)
        {
            _context.EventCategories.Remove(category);
            _context.SaveChanges();
        }

        public List<ServerEvent> GetAllEvents()
        {
            return _context.Events.Include(e => e.Category).OrderBy(e => e.StartsAt).ToList();
        }

        public ServerEvent? GetEventBySlug(string slug)
        {
            return _context.Events.Include(e => e.Category).FirstOrDefault(e => e.Slug == slug);
        }

        public ServerEvent? GetFeaturedEvent()
        {
            return _context.Events.Include(e => e.Category).FirstOrDefault(e => e.IsFeatured);
        }

        public List<ServerEvent> GetFeaturedEvents()
        {
            return _context.Events.Where(e => e.IsFeatured).ToList();
        }

        public void CreateEvent(ServerEvent serverEvent)
        {
            _context.Events.Add(serverEvent);
            _context.SaveChanges();
        }

        /// <summary>
        /// Les events et catégories viennent du contexte et restent suivis, les modifications sont détectées ici
        /// </summary>
        public void Update()
        {
            _context.SaveChanges();
        }

        public void DeleteEvent(ServerEvent serverEvent)
        {
            _context.Events.Remove(serverEvent);
            _context.SaveChanges();
        }

        public List<EventImage> GetImages(IEnumerable<string> fileNames)
        {
            List<string> names = fileNames.ToList();
            return _context.EventImages.Where(i => names.Contains(i.FileName)).ToList();
        }

        public List<EventImage> GetEventImages(int eventId)
        {
            return _context.EventImages.Where(i => i.EventId == eventId).ToList();
        }

        public List<EventImage> GetCategoryImages(int categoryId)
        {
            return _context.EventImages.Where(i => i.CategoryId == categoryId).ToList();
        }

        public List<EventImage> GetOrphanImages(DateTime uploadedBeforeUtc)
        {
            return _context.EventImages.Where(i => i.EventId == null && i.CategoryId == null && i.UploadedAtUtc < uploadedBeforeUtc).ToList();
        }

        public void CreateImage(EventImage image)
        {
            _context.EventImages.Add(image);
            _context.SaveChanges();
        }

        public void DeleteImages(IEnumerable<EventImage> images)
        {
            _context.EventImages.RemoveRange(images);
            _context.SaveChanges();
        }
    }
}
