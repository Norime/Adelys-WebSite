using Adelys_WebSite.ViewModels;

namespace Adelys_WebSite.BL.Interfaces
{
    public interface IHomeHeroBL
    {
        HomeHero GetHero();
        Task UpdateHero(HomeHeroFormViewModel form);
        void ResetHero();
    }
}
