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

        public virtual DbSet<Shop> Shops { get; set; }

        public virtual DbSet<ShopCategory> ShopCategories { get; set; }

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

            modelBuilder.Entity<Shop>(entity =>
            {
                entity.ToTable("site_shops");

                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.City).HasMaxLength(32);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Owner).HasMaxLength(64);
            });

            modelBuilder.Entity<ShopType>(entity =>
            {
                entity.ToTable("site_shop_types");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<ShopItem>(entity =>
            {
                entity.ToTable("site_shop_items");

                entity.Property(e => e.ItemId).HasMaxLength(64);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Currency).HasMaxLength(10);
            });

            modelBuilder.Entity<ShopCategory>(entity =>
            {
                entity.ToTable("site_shop_categories");

                entity.HasIndex(e => e.Name).IsUnique();

                entity.Property(e => e.Name).HasMaxLength(50);

                // Types de départ, les admins peuvent ensuite en ajouter ou en supprimer
                entity.HasData(
                    new ShopCategory { Id = 1, Name = "Matériaux de construction" },
                    new ShopCategory { Id = 2, Name = "Nourriture" },
                    new ShopCategory { Id = 3, Name = "Utilitaires" },
                    new ShopCategory { Id = 4, Name = "Redstone" });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
