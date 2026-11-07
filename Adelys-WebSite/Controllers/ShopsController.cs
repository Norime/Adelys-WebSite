using Adelys_WebSite.BL;
using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.SAL.Interface;
using Adelys_WebSite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Adelys_WebSite.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class ShopsController : Controller
    {
        private readonly ILogger<ShopsController> _logger;
        private readonly IShopBL _shopBL;
        private readonly IPlayerBL _playerBL;
        private readonly IMinecraftItemCatalog _itemCatalog;

        public ShopsController(ILogger<ShopsController> logger, IShopBL shopBL, IPlayerBL playerBL, IMinecraftItemCatalog itemCatalog)
        {
            _logger = logger;
            _shopBL = shopBL;
            _playerBL = playerBL;
            _itemCatalog = itemCatalog;
        }

        public IActionResult Index()
        {
            List<Shop> shops = _shopBL.GetAllShops();
            Dictionary<string, string> playerGroups = _playerBL.GetPlayerRanks()
                .GroupBy(r => r.Player.Username, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Rank, StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> ownerGroups = shops
                .Select(s => s.Owner)
                .Where(playerGroups.ContainsKey)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToDictionary(o => o, o => playerGroups[o], StringComparer.OrdinalIgnoreCase);

            return View("ShopsIndex", new ShopsViewModel
            {
                Shops = shops,
                Types = _shopBL.GetCategoryNames(),
                Owners = shops.Select(s => s.Owner).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(o => o, StringComparer.OrdinalIgnoreCase).ToList(),
                Groups = ownerGroups.Values.Distinct().OrderBy(g => g).ToList(),
                OwnerGroups = ownerGroups,
                CanManage = SiteRoles.CanEditContent(User)
            });
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpGet]
        public IActionResult Ajouter()
        {
            return ShowForm(new ShopFormViewModel());
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        public IActionResult Ajouter(ShopFormViewModel model)
        {
            ValidateForm(model);
            if (!ModelState.IsValid)
            {
                return ShowForm(model);
            }

            _shopBL.CreateShop(model);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpGet]
        public IActionResult Modifier(int id)
        {
            Shop? shop = _shopBL.GetShop(id);
            if (shop == null)
            {
                return NotFound();
            }
            return ShowForm(new ShopFormViewModel
            {
                Id = shop.Id,
                Name = shop.Name,
                City = shop.City,
                Types = shop.Types.Select(t => t.Name).ToList(),
                Description = shop.Description,
                Owner = shop.Owner,
                X = shop.X,
                Y = shop.Y,
                Z = shop.Z,
                Items = shop.Items.Select(i => new ShopItemFormViewModel
                {
                    ItemId = i.ItemId,
                    Name = _itemCatalog.Find(i.ItemId)?.Name ?? i.Name,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Currency = i.Currency
                }).ToList()
            });
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        public IActionResult Modifier(int id, ShopFormViewModel model)
        {
            Shop? shop = _shopBL.GetShop(id);
            if (shop == null)
            {
                return NotFound();
            }

            model.Id = id;
            ValidateForm(model);
            if (!ModelState.IsValid)
            {
                return ShowForm(model);
            }

            _shopBL.UpdateShop(shop, model);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        public IActionResult Supprimer(int id)
        {
            Shop? shop = _shopBL.GetShop(id);
            if (shop != null)
            {
                _shopBL.DeleteShop(shop);
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Liste des items pour le sélecteur du formulaire
        /// </summary>
        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpGet]
        public IActionResult Items()
        {
            return Json(_itemCatalog.Items.Select(i => new { id = i.Id, name = i.Name, icon = _itemCatalog.GetIconUrl(i.Id) }));
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpGet]
        public IActionResult Types()
        {
            return ShowCategories(new ShopCategoriesViewModel());
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        public IActionResult AjouterType(ShopCategoriesViewModel model)
        {
            if (ModelState.IsValid && _shopBL.CategoryExists(model.NewName))
            {
                ModelState.AddModelError(nameof(model.NewName), "Ce type existe déjà");
            }
            if (!ModelState.IsValid)
            {
                return ShowCategories(model);
            }

            _shopBL.CreateCategory(model.NewName);
            return RedirectToAction(nameof(Types));
        }

        [Authorize(Roles = SiteRoles.ContentEditors)]
        [HttpPost]
        public IActionResult SupprimerType(int id)
        {
            ShopCategory? category = _shopBL.GetCategory(id);
            if (category != null)
            {
                _shopBL.DeleteCategory(category);
            }
            return RedirectToAction(nameof(Types));
        }

        private IActionResult ShowCategories(ShopCategoriesViewModel model)
        {
            model.Categories = _shopBL.GetAllCategories();
            model.ShopCounts = _shopBL.CountShopsByCategory();
            return View("ShopTypes", model);
        }

        private IActionResult ShowForm(ShopFormViewModel model)
        {
            model.Players = _playerBL.GetAllPlayers().Select(p => p.Username).OrderBy(u => u).ToList();
            model.AvailableTypes = _shopBL.GetCategoryNames();
            return View("ShopForm", model);
        }

        private void ValidateForm(ShopFormViewModel model)
        {
            if (!ShopCatalog.Cities.Contains(model.City))
            {
                ModelState.AddModelError(nameof(model.City), "Choisis une ville");
            }
            if (model.Types.Count == 0 || model.Types.Except(_shopBL.GetCategoryNames()).Any())
            {
                ModelState.AddModelError(nameof(model.Types), "Choisis au moins un type");
            }
            ValidateItems(model);
        }

        private void ValidateItems(ShopFormViewModel model)
        {
            List<ShopItemFormViewModel> items = model.Items.Where(i => !i.IsEmpty).ToList();
            if (items.Count > 0 && _itemCatalog.Version == null)
            {
                ModelState.AddModelError(nameof(model.Items), "La liste des items Minecraft n'est pas encore prête, réessaie dans quelques minutes");
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                ShopItemFormViewModel item = items[i];
                string row = $"Item {i + 1} : ";
                if (_itemCatalog.Find(item.ItemId) == null)
                {
                    ModelState.AddModelError(nameof(model.Items), row + "choisis l'item dans la liste");
                }
                if (item.Quantity is not (>= 1 and <= 100000))
                {
                    ModelState.AddModelError(nameof(model.Items), row + "la quantité doit être comprise entre 1 et 100 000");
                }
                if (item.Price is not (>= 1 and <= 1000000))
                {
                    ModelState.AddModelError(nameof(model.Items), row + "le prix doit être compris entre 1 et 1 000 000");
                }
                if (!ShopCurrencies.All.Contains(item.Currency))
                {
                    ModelState.AddModelError(nameof(model.Items), row + "choisis cuivre, fer ou or");
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult GetShopsFromCityPartialView(string cityName)
        {
            //_dataStorageBL.GetListShops();
            ShopsViewModel viewModel = new()
            {
            };
            return PartialView("~/Views/Shops/PartialViews/_displayListeShops.cshtml", viewModel);
        }
    }
}