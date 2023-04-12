using Adelys_WebSite.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace Adelys_WebSite.BL
{
    public class DataStorageBL : IDataStorageBL
    {
        public List<ShopModel> listShops { get; set; }
        public List<PlayerModel> listPlayers { get; set; }

    }

    protected void DataStorageBL(IDataStorageBL dataStorageBL)
    {
        listPlayers = new();
    }

    /// <summary>
    /// Récupère la liste des shops dans le json
    /// </summary>
    //public void GetListShops()
    //        {
    //            listShops = new();
    //            string fileName = "~/content/DataJson/shops.json";
    //            //using FileStream openStream = File.OpenRead(fileName);
    //            //List<ShopModel>? shops =
    //            //    await JsonSerializer.DeserializeAsync<List<ShopModel>>(openStream);
    //            List<ShopModel> jsonString = JsonSerializer.Deserialize<List<ShopModel>>(fileName);
    //            listShops = jsonString;
    //        }

    public void GetPlayers()
    {
        var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
        using var context = new DbContextModel(optionsBuilder.Options);
        var customers = context.Players.ToList();
        var orders = context.Orders.Where(o => o.CustomerID == 1).ToList();
    }

    private MySqlConnection GetConnection()
    {
        return new MySqlConnection(Environment.GetEnvironmentVariable("MYSQL_CONNECTIONSTRING"));
    }
}
