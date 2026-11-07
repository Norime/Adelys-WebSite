using Adelys_WebSite.BL;
using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Adelys_WebSite.ViewModels
{
    public class ShopsViewModel
    {
        public IPlayerBL PlayerBL { get; set; }
        public List<Shop> Shops { get; set; } = new();
        public List<string> Types { get; set; } = new();
        public List<string> Owners { get; set; } = new();
        public List<string> Groups { get; set; } = new();
        /// <summary>
        /// Groupe de chaque propriétaire, retrouvé par son pseudo dans LuckPerms, sans tenir compte des majuscules
        /// </summary>
        public Dictionary<string, string> OwnerGroups { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public bool CanManage { get; set; }
    }

    public class ShopCategoriesViewModel
    {
        public List<ShopCategory> Categories { get; set; } = new();
        public Dictionary<string, int> ShopCounts { get; set; } = new();

        [Required(ErrorMessage = "Le nom du type est obligatoire")]
        [StringLength(50, ErrorMessage = "Le nom du type fait 50 caractères maximum")]
        [Display(Name = "Nouveau type")]
        public string NewName { get; set; } = string.Empty;
    }

    public class ShopFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom fait 100 caractères maximum")]
        [Display(Name = "Nom du commerce")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choisis une ville")]
        [Display(Name = "Ville")]
        public string City { get; set; } = string.Empty;

        public List<string> Types { get; set; } = new();

        [StringLength(1000, ErrorMessage = "Le descriptif fait 1000 caractères maximum")]
        [Display(Name = "Descriptif (facultatif)")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Le propriétaire est obligatoire")]
        [StringLength(64, ErrorMessage = "Le propriétaire fait 64 caractères maximum")]
        [Display(Name = "Propriétaire")]
        public string Owner { get; set; } = string.Empty;

        [Required(ErrorMessage = "La position X est obligatoire")]
        public int? X { get; set; }

        [Required(ErrorMessage = "La position Y est obligatoire")]
        public int? Y { get; set; }

        [Required(ErrorMessage = "La position Z est obligatoire")]
        public int? Z { get; set; }

        public List<ShopItemFormViewModel> Items { get; set; } = new();

        [BindNever]
        public List<string> Players { get; set; } = new();

        [BindNever]
        public List<string> AvailableTypes { get; set; } = new();
    }

    public class ShopItemFormViewModel
    {
        public string? ItemId { get; set; }

        /// <summary>
        /// Texte tapé dans le champ, renvoyé tel quel si le formulaire doit être corrigé
        /// </summary>
        public string? Name { get; set; }

        public int? Quantity { get; set; }

        public int? Price { get; set; }

        public string? Currency { get; set; }

        public bool IsEmpty => string.IsNullOrWhiteSpace(ItemId) && string.IsNullOrWhiteSpace(Name) && Quantity == null && Price == null;
    }

    public class ShopItemRowViewModel
    {
        /// <summary>
        /// Position de la ligne, absente dans le modèle de ligne que le script copie puis numérote
        /// </summary>
        public int? Index { get; set; }
        public ShopItemFormViewModel Item { get; set; } = new();
        public string? IconUrl { get; set; }

        public string? FieldName(string field)
        {
            return Index != null ? $"Items[{Index}].{field}" : null;
        }
    }
}
