using Adelys_WebSite.Models;

namespace Adelys_WebSite.BL.Interfaces
{
    public interface IUserPermissionBL
    {
        List<LuckpermsUserPermission> GetAllUsersPermissions();
        LuckpermsUserPermission GetUserPermission(int id);

        List<LuckpermsUserPermission> GetUserPermissionByPlayerUuid(string uuid);
    }
}
