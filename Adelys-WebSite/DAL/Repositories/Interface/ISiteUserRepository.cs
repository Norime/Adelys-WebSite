using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories.Interfaces
{
    public interface ISiteUserRepository
    {
        List<SiteUser> GetAll();
        SiteUser? GetById(int id);
        SiteUser? GetByUsername(string username);
        void Create(SiteUser user);
        void Update(SiteUser user);
        void Delete(SiteUser user);
    }
}
