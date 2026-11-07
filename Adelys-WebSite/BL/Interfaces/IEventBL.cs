using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;

namespace Adelys_WebSite.BL.Interfaces
{
    public interface IEventBL
    {
        List<EventCategory> GetAllCategories();
        EventCategory? GetCategory(int id);
        bool IsCategoryNameTaken(string name, int? exceptId);
        EventCategory? GetCategory(string slug);
        Task<EventCategory> CreateCategory(EventCategoryFormViewModel form);
        Task UpdateCategory(EventCategory category, EventCategoryFormViewModel form);
        void DeleteCategory(EventCategory category);

        List<ServerEvent> GetAllEvents();
        ServerEvent? GetEvent(string slug);
        ServerEvent? GetFeaturedEvent(DateTime now);
        ServerEvent? GetCurrentEvent(List<ServerEvent> events, DateTime now);
        Task<ServerEvent> CreateEvent(EventFormViewModel form, string createdBy);
        Task UpdateEvent(ServerEvent serverEvent, EventFormViewModel form);
        void DeleteEvent(ServerEvent serverEvent);

        Task<string> SaveImage(IFormFile file, string extension);
    }
}
