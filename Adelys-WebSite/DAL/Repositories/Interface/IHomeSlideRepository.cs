using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories.Interfaces
{
    public interface IHomeSlideRepository
    {
        List<HomeSlide> GetAll();
        HomeSlide? GetById(int id);
        void Create(HomeSlide slide);
        void Update();
        void Delete(HomeSlide slide);
    }
}
