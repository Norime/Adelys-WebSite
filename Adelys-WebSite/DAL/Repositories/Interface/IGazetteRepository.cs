using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories.Interfaces
{
    public interface IGazetteRepository
    {
        List<Gazette> GetAll();
        Gazette? GetBySlug(string slug);
        void Create(Gazette gazette);
        void Update(Gazette gazette);
        void Delete(Gazette gazette);
    }
}
