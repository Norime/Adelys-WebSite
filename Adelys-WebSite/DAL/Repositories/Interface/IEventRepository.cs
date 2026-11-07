using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories.Interfaces
{
    public interface IEventRepository
    {
        List<EventCategory> GetAllCategories();
        EventCategory? GetCategory(int id);
        EventCategory? GetCategoryBySlug(string slug);
        bool IsCategoryNameTaken(string name, int? exceptId);
        void CreateCategory(EventCategory category);
        void DeleteCategory(EventCategory category);

        List<ServerEvent> GetAllEvents();
        ServerEvent? GetEventBySlug(string slug);
        ServerEvent? GetFeaturedEvent();
        List<ServerEvent> GetFeaturedEvents();
        void CreateEvent(ServerEvent serverEvent);
        void Update();
        void DeleteEvent(ServerEvent serverEvent);

        List<EventImage> GetImages(IEnumerable<string> fileNames);
        List<EventImage> GetEventImages(int eventId);
        List<EventImage> GetCategoryImages(int categoryId);
        List<EventImage> GetOrphanImages(DateTime uploadedBeforeUtc);
        void CreateImage(EventImage image);
        void DeleteImages(IEnumerable<EventImage> images);
    }
}
