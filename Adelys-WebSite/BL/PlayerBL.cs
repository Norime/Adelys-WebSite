using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.DAL.UnitOfWork.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.SAL.Interface;
using System.Threading.Tasks;

namespace Adelys_WebSite.BL
{
    public class PlayerBL : BaseBL<LuckpermsPlayer>, IPlayerBL
    {
        protected override IPlayerRepository Repository => _unitOfWork.PlayerRepositoryDAO;
        private readonly IServiceMojangAccess _mojangApi;
        private readonly ICrafatarAccess _crafatarApi;

        public PlayerBL(IUnitOfWork unitOfWork, IServiceMojangAccess mojangAccess, ICrafatarAccess crafatarApi) : base(unitOfWork)
        {
            _mojangApi = mojangAccess;
            _crafatarApi = crafatarApi;
        }

        /// <summary>
        /// Récupère tout les customers via les repository customer
        /// </summary>
        /// <returns></returns>
        public List<LuckpermsPlayer> GetAllPlayers()
        {
            return Repository.GetAllPlayers();
        }

        /// <summary>
        /// Récupère un customer en fonction de son identifiant à partir du repository générale
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LuckpermsPlayer GetPlayer(int id)
        {
            return Repository.GetById(id);
        }

        public string GetPlayerSkin(string uuid)
        {
            Task<string> playerSkin = _mojangApi.GetSkinData(uuid);
            playerSkin.Wait();
            return playerSkin.Result;
        }

        public string GetPlayerSkinImg(string uuid)
        {
            Task<string> playerSkin = _crafatarApi.GetPlayerSkin(uuid);
            playerSkin.Wait();
            return playerSkin.Result;
        }
    }
}
