using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;

namespace Adelys_WebSite.Models
{
    public class DbContextModel : DbContext
    {
        public DbSet<PlayerModel> Players { get; set; }

        public DbContextModel(DbContextOptions<DbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
