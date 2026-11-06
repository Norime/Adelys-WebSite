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

        public virtual DbSet<Gazette> Gazettes { get; set; }

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

            modelBuilder.Entity<Gazette>(entity =>
            {
                entity.ToTable("site_gazettes");

                entity.HasIndex(e => e.Slug).IsUnique();

                entity.Property(e => e.Title).HasMaxLength(150);
                entity.Property(e => e.Slug).HasMaxLength(170);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.PublishedOn).HasColumnType("date");
                entity.Property(e => e.PdfFileName).HasMaxLength(64);
                entity.Property(e => e.CoverFileName).HasMaxLength(64);
                entity.Property(e => e.UploadedBy).HasMaxLength(32);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
