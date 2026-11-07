using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories
{
    public class GazetteRepository : IGazetteRepository
    {
        private readonly SiteDbContext _context;

        public GazetteRepository(SiteDbContext context)
        {
            _context = context;
        }

        public List<Gazette> GetAll()
        {
            return _context.Gazettes.OrderByDescending(g => g.PublishedOn).ThenByDescending(g => g.Id).ToList();
        }

        public Gazette? GetBySlug(string slug)
        {
            return _context.Gazettes.FirstOrDefault(g => g.Slug == slug);
        }

        public void Create(Gazette gazette)
        {
            _context.Gazettes.Add(gazette);
            _context.SaveChanges();
        }

        public void Update(Gazette gazette)
        {
            _context.Gazettes.Update(gazette);
            _context.SaveChanges();
        }

        public void Delete(Gazette gazette)
        {
            _context.Gazettes.Remove(gazette);
            _context.SaveChanges();
        }
    }
}
