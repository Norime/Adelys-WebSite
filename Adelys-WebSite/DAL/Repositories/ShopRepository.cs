using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;
using Microsoft.EntityFrameworkCore;

namespace Adelys_WebSite.DAL.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly SiteDbContext _context;

        public ShopRepository(SiteDbContext context)
        {
            _context = context;
        }

        public List<Shop> GetAll()
        {
            return WithDetails().OrderBy(s => s.Name).ToList();
        }

        public Shop? GetById(int id)
        {
            return WithDetails().FirstOrDefault(s => s.Id == id);
        }

        public void Create(Shop shop)
        {
            _context.Shops.Add(shop);
            _context.SaveChanges();
        }

        /// <summary>
        /// Le commerce vient de GetById et reste suivi par le contexte, ses types et items remplacés sont détectés ici
        /// </summary>
        public void Update()
        {
            _context.SaveChanges();
        }

        public void Delete(Shop shop)
        {
            _context.Shops.Remove(shop);
            _context.SaveChanges();
        }

        public List<ShopCategory> GetAllCategories()
        {
            return _context.ShopCategories.OrderBy(c => c.Name).ToList();
        }

        public ShopCategory? GetCategory(int id)
        {
            return _context.ShopCategories.FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// La comparaison suit la collation MySQL, comme l'index unique : « nourriture » existe déjà si « Nourriture » existe
        /// </summary>
        public bool CategoryExists(string name)
        {
            return _context.ShopCategories.Any(c => c.Name == name);
        }

        public Dictionary<string, int> CountShopsByCategory()
        {
            return _context.Set<ShopType>()
                .GroupBy(t => t.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToDictionary(g => g.Name, g => g.Count);
        }

        public void CreateCategory(ShopCategory category)
        {
            _context.ShopCategories.Add(category);
            _context.SaveChanges();
        }

        /// <summary>
        /// Retire aussi le type des commerces qui l'avaient, dans la même transaction
        /// </summary>
        public void DeleteCategory(ShopCategory category)
        {
            _context.Set<ShopType>().RemoveRange(_context.Set<ShopType>().Where(t => t.Name == category.Name));
            _context.ShopCategories.Remove(category);
            _context.SaveChanges();
        }

        private IQueryable<Shop> WithDetails()
        {
            return _context.Shops.Include(s => s.Types).Include(s => s.Items.OrderBy(i => i.Id)).AsSplitQuery();
        }
    }
}
