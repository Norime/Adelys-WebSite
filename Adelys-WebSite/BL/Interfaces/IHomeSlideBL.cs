using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;

namespace Adelys_WebSite.BL.Interfaces
{
    public interface IHomeSlideBL
    {
        List<HomeSlide> GetAllSlides();
        HomeSlide? GetSlide(int id);
        Task CreateSlide(HomeSlideFormViewModel form);
        Task UpdateSlide(HomeSlide slide, HomeSlideFormViewModel form);
        void DeleteSlide(HomeSlide slide);
        void Move(HomeSlide slide, int direction);
    }
}
