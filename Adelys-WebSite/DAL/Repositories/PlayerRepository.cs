using Adelys_WebSite.DAL;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories
{
    public class PlayerRepository : Repository<LuckpermsPlayer>, IPlayerRepository
    {

        public PlayerRepository(MySqlDbContext context) : base(context)
        {
        }

        List<LuckpermsPlayer> IPlayerRepository.GetAllPlayers()
        {
            var list = _context.LuckpermsPlayers.ToList();
            return list;
        }

        //public void CreateCustomer(Customer customer)
        //{
        //    var t = _context.Customer.Add(customer);
        //    _context.SaveChanges();
        //}
    }
}
