using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories
{
    public class SiteUserRepository : ISiteUserRepository
    {
        private readonly SiteDbContext _context;

        public SiteUserRepository(SiteDbContext context)
        {
            _context = context;
        }

        public List<SiteUser> GetAll()
        {
            return _context.SiteUsers.OrderBy(u => u.Username).ToList();
        }

        public SiteUser? GetById(int id)
        {
            return _context.SiteUsers.Find(id);
        }

        public SiteUser? GetByUsername(string username)
        {
            return _context.SiteUsers.FirstOrDefault(u => u.Username == username);
        }

        public void Create(SiteUser user)
        {
            _context.SiteUsers.Add(user);
            _context.SaveChanges();
        }

        public void Update(SiteUser user)
        {
            _context.SiteUsers.Update(user);
            _context.SaveChanges();
        }

        public void Delete(SiteUser user)
        {
            _context.SiteUsers.Remove(user);
            _context.SaveChanges();
        }
    }
}
