using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories.Interfaces
{
    public interface IShopRepository
    {
        List<Shop> GetAll();
        Shop? GetById(int id);
        void Create(Shop shop);
        void Update();
        void Delete(Shop shop);
        List<ShopCategory> GetAllCategories();
        ShopCategory? GetCategory(int id);
        bool CategoryExists(string name);
        Dictionary<string, int> CountShopsByCategory();
        void CreateCategory(ShopCategory category);
        void DeleteCategory(ShopCategory category);
    }
}
