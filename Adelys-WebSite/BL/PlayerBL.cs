using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.DAL.UnitOfWork.Interfaces;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.BL
{
    public class PlayerBL : BaseBL<LuckpermsPlayer>, IPlayerBL
    {
        protected override IPlayerRepository Repository => _unitOfWork.PlayerRepositoryDAO;

        public PlayerBL(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
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

        public void CreatePlayer(LuckpermsPlayer player)
        {
            try
            {
                Repository.Create(player);
            }catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public void UpdatePlayer(LuckpermsPlayer player)
        {
            Repository.Update(player);
        }

        public void DeletePlayer(LuckpermsPlayer player)
        {
            Repository.Delete(player);
        }
    }
}
