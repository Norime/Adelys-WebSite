using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.SAL.Interface;
using Adelys_WebSite.ViewModels;

namespace Adelys_WebSite.BL
{
    public class ShopBL : IShopBL
    {
        private readonly IShopRepository _repository;
        private readonly IMinecraftItemCatalog _itemCatalog;

        public ShopBL(IShopRepository repository, IMinecraftItemCatalog itemCatalog)
        {
            _repository = repository;
            _itemCatalog = itemCatalog;
        }

        public List<Shop> GetAllShops()
        {
            return _repository.GetAll();
        }

        public Shop? GetShop(int id)
        {
            return _repository.GetById(id);
        }

        public void CreateShop(ShopFormViewModel form)
        {
            Shop shop = new Shop { CreatedAtUtc = DateTime.UtcNow };
            ApplyForm(shop, form);
            _repository.Create(shop);
        }

        public void UpdateShop(Shop shop, ShopFormViewModel form)
        {
            ApplyForm(shop, form);
            _repository.Update();
        }

        public void DeleteShop(Shop shop)
        {
            _repository.Delete(shop);
        }

        public List<ShopCategory> GetAllCategories()
        {
            return _repository.GetAllCategories();
        }

        public List<string> GetCategoryNames()
        {
            return _repository.GetAllCategories().Select(c => c.Name).ToList();
        }

        public ShopCategory? GetCategory(int id)
        {
            return _repository.GetCategory(id);
        }

        public bool CategoryExists(string name)
        {
            return _repository.CategoryExists(name.Trim());
        }

        public Dictionary<string, int> CountShopsByCategory()
        {
            return _repository.CountShopsByCategory();
        }

        public void CreateCategory(string name)
        {
            _repository.CreateCategory(new ShopCategory { Name = name.Trim() });
        }

        public void DeleteCategory(ShopCategory category)
        {
            _repository.DeleteCategory(category);
        }

        private void ApplyForm(Shop shop, ShopFormViewModel form)
        {
            shop.Name = form.Name.Trim();
            shop.City = form.City;
            shop.Description = form.Description?.Trim();
            shop.Owner = form.Owner.Trim();
            shop.X = form.X!.Value;
            shop.Y = form.Y!.Value;
            shop.Z = form.Z!.Value;

            shop.Types.Clear();
            shop.Types.AddRange(form.Types.Distinct().Select(t => new ShopType { Name = t }));

            // Une ligne laissée vide dans le formulaire n'est pas un item, le nom vient du catalogue et non du texte tapé
            shop.Items.Clear();
            shop.Items.AddRange(form.Items
                .Where(i => !i.IsEmpty)
                .Select(i => new ShopItem
                {
                    ItemId = i.ItemId!,
                    Name = _itemCatalog.Find(i.ItemId)!.Name,
                    Quantity = i.Quantity!.Value,
                    Price = i.Price!.Value,
                    Currency = i.Currency!
                }));
        }
    }
}
