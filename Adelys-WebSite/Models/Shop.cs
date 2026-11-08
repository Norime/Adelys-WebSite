namespace Adelys_WebSite.Models
{
    public class Shop
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Owner { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public List<ShopType> Types { get; set; } = new();
        public List<ShopItem> Items { get; set; } = new();
    }

    /// <summary>
    /// Type coché sur un commerce, son nom reprend celui d'un ShopCategory
    /// </summary>
    public class ShopType
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ShopItem
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        /// <summary>
        /// Identifiant Minecraft, par exemple oak_planks, vide pour un item saisi avant le catalogue
        /// </summary>
        public string ItemId { get; set; } = string.Empty;
        /// <summary>
        /// Nom français au moment de l'enregistrement, affiché si l'item disparaît du catalogue
        /// </summary>
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string Currency { get; set; } = ShopCurrencies.Copper;
    }

    /// <summary>
    /// Monnaies du serveur, de la plus faible à la plus forte
    /// </summary>
    public static class ShopCurrencies
    {
        public const string Copper = "cuivre";
        public const string Iron = "fer";
        public const string Gold = "or";
        public static readonly string[] All = { Copper, Iron, Gold };

        public static string GetLabel(string currency)
        {
            return currency switch { Iron => "Fer", Gold => "Or", _ => "Cuivre" };
        }

        public static string GetIconItemId(string currency)
        {
            return currency switch { Iron => "raw_iron", Gold => "raw_gold", _ => "raw_copper" };
        }
    }

    /// <summary>
    /// Type de commerce proposé dans le formulaire et le filtre, géré par les admins
    /// </summary>
    public class ShopCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public static class ShopCatalog
    {
        public static readonly string[] Cities = { "Adelys", "Pristo", "Mazadir", "Grenat" };
    }
}
