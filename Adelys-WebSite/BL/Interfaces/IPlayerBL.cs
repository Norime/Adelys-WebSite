using Adelys_WebSite.Models;

namespace Adelys_WebSite.BL.Interfaces
{
    public interface IPlayerBL
    {
        List<LuckpermsPlayer> GetAllPlayers();
        List<PlayerRank> GetPlayerRanks();
        LuckpermsPlayer GetPlayer(int id);
        string GetPlayerSkin(string uuid);
        string GetPlayerSkinImg(string uuid);
    }
}
