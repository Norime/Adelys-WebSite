using Adelys_WebSite.Models;
using Microsoft.EntityFrameworkCore;

namespace Adelys_WebSite.DAL
{
    /// <summary>
    /// Données propres au site, séparées des tables LuckPerms qui restent en lecture seule
    /// </summary>
    public class SiteDbContext : DbContext
    {
        public const string MigrationsHistoryTable = "__site_migrations";

        public SiteDbContext(DbContextOptions<SiteDbContext> options)
        : base(options)
        {
        }

        public virtual DbSet<SiteUser> SiteUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SiteUser>(entity =>
            {
                entity.ToTable("site_users");

                entity.HasIndex(e => e.Username).IsUnique();

                entity.Property(e => e.Username).HasMaxLength(32);
                entity.Property(e => e.PasswordHash).HasMaxLength(255);
                entity.Property(e => e.Role).HasMaxLength(32);
                entity.Property(e => e.SecurityStamp).HasMaxLength(64);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
