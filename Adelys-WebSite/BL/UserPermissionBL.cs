using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.DAL.UnitOfWork.Interfaces;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.BL
{
    public class UserPermissionBL : BaseBL<LuckpermsUserPermission>, IUserPermissionBL
    {
        protected override IUserPermissionRepository Repository => _unitOfWork.UserPermissionRepositoryDAO;

        public UserPermissionBL(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        /// Récupère tout les permissions des utilisateurs via les repository UserPermission
        /// </summary>
        /// <returns></returns>
        public List<LuckpermsUserPermission> GetAllUsersPermissions()
        {
            return Repository.GetAllUsersPermissions();
        }

        /// <summary>
        /// Récupère un customer en fonction de son identifiant à partir du repository générale
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LuckpermsUserPermission GetUserPermission(int id)
        {
            return Repository.GetById(id);
        }

        public List<LuckpermsUserPermission> GetUserPermissionByPlayerUuid(string uuid)
        {
            return Repository.GetUserPermissionByPlayerUuid(uuid);
        }
    }
}
