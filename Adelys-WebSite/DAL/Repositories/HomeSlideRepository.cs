using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories
{
    public class HomeSlideRepository : IHomeSlideRepository
    {
        private readonly SiteDbContext _context;

        public HomeSlideRepository(SiteDbContext context)
        {
            _context = context;
        }

        public List<HomeSlide> GetAll()
        {
            return _context.HomeSlides.OrderBy(s => s.SortOrder).ThenBy(s => s.Id).ToList();
        }

        public HomeSlide? GetById(int id)
        {
            return _context.HomeSlides.FirstOrDefault(s => s.Id == id);
        }

        public void Create(HomeSlide slide)
        {
            _context.HomeSlides.Add(slide);
            _context.SaveChanges();
        }

        /// <summary>
        /// Les images viennent du contexte et restent suivies, les modifications sont détectées ici
        /// </summary>
        public void Update()
        {
            _context.SaveChanges();
        }

        public void Delete(HomeSlide slide)
        {
            _context.HomeSlides.Remove(slide);
            _context.SaveChanges();
        }
    }
}
