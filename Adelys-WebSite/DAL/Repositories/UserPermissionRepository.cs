using Adelys_WebSite.DAL;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories
{
    public class UserPermissionRepository : Repository<LuckpermsUserPermission>, IUserPermissionRepository
    {

        public UserPermissionRepository(MySqlDbContext context) : base(context)
        {
        }

        List<LuckpermsUserPermission> IUserPermissionRepository.GetAllUsersPermissions()
        {
            var list = _context.LuckpermsUserPermissions.ToList();
            return list;
        }
        

        List<LuckpermsUserPermission> IUserPermissionRepository.GetUserPermissionByPlayerUuid(string uuid)
        {
            var list = _context.LuckpermsUserPermissions.Where(p => p.Uuid == uuid).ToList();
            return list;
        }
        //public void CreateCustomer(Customer customer)
        //{
        //    var t = _context.Customer.Add(customer);
        //    _context.SaveChanges();
        //}
    }
}
