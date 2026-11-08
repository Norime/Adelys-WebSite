using Adelys_WebSite.DAL.Repositories.Interface;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories.Interfaces
{
    public interface IUserPermissionRepository : IRepository<LuckpermsUserPermission>
    {
        List<LuckpermsUserPermission> GetAllUsersPermissions();

        List<LuckpermsUserPermission> GetUserPermissionByPlayerUuid(string uuid);
    }
}
