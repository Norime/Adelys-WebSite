using Adelys_WebSite.DAL.Repositories.Interface;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories.Interfaces
{
    public interface IPlayerRepository : IRepository<LuckpermsPlayer>
    {
        List<LuckpermsPlayer> GetAllPlayers();
    }
}
