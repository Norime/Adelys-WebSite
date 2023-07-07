using Adelys_WebSite.Models;

namespace Adelys_WebSite.BL.Interfaces
{
    public interface IPlayerBL
    {
        List<LuckpermsPlayer> GetAllPlayers();
        LuckpermsPlayer GetPlayer(int id);
        void CreatePlayer(LuckpermsPlayer player);
        void UpdatePlayer(LuckpermsPlayer player);
        void DeletePlayer(LuckpermsPlayer player);
    }
}
