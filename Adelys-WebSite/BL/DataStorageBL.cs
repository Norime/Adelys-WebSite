using Adelys_WebSite.Models;
using System.Text.Json;

namespace Adelys_WebSite.BL
{
    public class DataStorageBL : IDataStorageBL
    {
        public List<ShopModel> listShops { get; set; } 
        /// <summary>
        /// Récupère la liste des shops dans le json
        /// </summary>
        public void GetListShops()
        {
            listShops = new();
            string fileName = "~/content/DataJson/shops.json";
            //using FileStream openStream = File.OpenRead(fileName);
            //List<ShopModel>? shops =
            //    await JsonSerializer.DeserializeAsync<List<ShopModel>>(openStream);
            List<ShopModel> jsonString = JsonSerializer.Deserialize<List<ShopModel>>(fileName);
            listShops = jsonString;
        }
    } 
}
