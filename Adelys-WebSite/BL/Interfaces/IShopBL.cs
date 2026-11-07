using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;

namespace Adelys_WebSite.BL.Interfaces
{
    public interface IShopBL
    {
        List<Shop> GetAllShops();
        Shop? GetShop(int id);
        void CreateShop(ShopFormViewModel form);
        void UpdateShop(Shop shop, ShopFormViewModel form);
        void DeleteShop(Shop shop);
        List<ShopCategory> GetAllCategories();
        List<string> GetCategoryNames();
        ShopCategory? GetCategory(int id);
        bool CategoryExists(string name);
        Dictionary<string, int> CountShopsByCategory();
        void CreateCategory(string name);
        void DeleteCategory(ShopCategory category);
    }
}
