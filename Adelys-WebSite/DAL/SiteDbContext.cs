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

        public virtual DbSet<Law> Laws { get; set; }

        public virtual DbSet<ParliamentMeeting> ParliamentMeetings { get; set; }

        public virtual DbSet<ParliamentImage> ParliamentImages { get; set; }

        public virtual DbSet<EventCategory> EventCategories { get; set; }

        public virtual DbSet<ServerEvent> Events { get; set; }

        public virtual DbSet<EventImage> EventImages { get; set; }

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

            modelBuilder.Entity<Law>(entity =>
            {
                entity.ToTable("site_laws");

                entity.HasIndex(e => e.Number).IsUnique();

                entity.Property(e => e.Number).HasMaxLength(10);
                entity.Property(e => e.Content).HasMaxLength(5000);

                entity.HasMany(e => e.Amendments).WithOne().HasForeignKey(a => a.LawId).OnDelete(DeleteBehavior.Cascade);

                entity.HasData(InitialLaws.Laws);
            });

            modelBuilder.Entity<LawAmendment>(entity =>
            {
                entity.ToTable("site_law_amendments");

                entity.Property(e => e.AmendedOn).HasColumnType("date");
                entity.Property(e => e.Note).HasMaxLength(300);

                entity.HasData(InitialLaws.Amendments);
            });

            modelBuilder.Entity<ParliamentMeeting>(entity =>
            {
                entity.ToTable("site_parliament_meetings");

                entity.Property(e => e.Title).HasMaxLength(150);
                entity.Property(e => e.HeldOn).HasColumnType("date");
                entity.Property(e => e.Content).HasColumnType("mediumtext");
                entity.Property(e => e.CreatedBy).HasMaxLength(32);
            });

            modelBuilder.Entity<ParliamentImage>(entity =>
            {
                entity.ToTable("site_parliament_images");

                entity.HasIndex(e => e.FileName).IsUnique();

                entity.Property(e => e.FileName).HasMaxLength(64);

                entity.HasOne<ParliamentMeeting>().WithMany().HasForeignKey(e => e.MeetingId).OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<EventCategory>(entity =>
            {
                entity.ToTable("site_event_categories");

                entity.HasIndex(e => e.Slug).IsUnique();

                entity.Property(e => e.Name).HasMaxLength(80);
                entity.Property(e => e.Slug).HasMaxLength(100);
                entity.Property(e => e.Summary).HasMaxLength(500);
                entity.Property(e => e.Description).HasColumnType("mediumtext");
                entity.Property(e => e.CoverFileName).HasMaxLength(64);

                // Les familles d'events du serveur, présentées ensuite par les admins
                entity.HasData(
                    new EventCategory { Id = 1, Name = "Les jeux de la mort", Slug = "jeux-de-la-mort", SortOrder = 2, Summary = "Le principe des jeux de la mort, leurs règles et toutes les éditions passées." },
                    new EventCategory { Id = 2, Name = "Les events saisonniers", Slug = "saisonniers", SortOrder = 1, Summary = "Les events organisés au fil des saisons et toutes leurs éditions." },
                    new EventCategory { Id = 3, Name = "Les events spéciaux", Slug = "speciaux", SortOrder = 3, Summary = "Les events uniques, organisés pour une occasion particulière." });
            });

            modelBuilder.Entity<ServerEvent>(entity =>
            {
                entity.ToTable("site_events");

                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.StartsAt);

                entity.Property(e => e.Title).HasMaxLength(150);
                entity.Property(e => e.Slug).HasMaxLength(170);
                entity.Property(e => e.Location).HasMaxLength(150);
                entity.Property(e => e.Summary).HasMaxLength(300);
                entity.Property(e => e.Content).HasColumnType("mediumtext");
                entity.Property(e => e.CoverFileName).HasMaxLength(64);
                entity.Property(e => e.CreatedBy).HasMaxLength(32);

                // Une catégorie ne se supprime qu'une fois vidée de ses events
                entity.HasOne(e => e.Category).WithMany(c => c.Events).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EventImage>(entity =>
            {
                entity.ToTable("site_event_images");

                entity.HasIndex(e => e.FileName).IsUnique();

                entity.Property(e => e.FileName).HasMaxLength(64);

                entity.HasOne<ServerEvent>().WithMany().HasForeignKey(e => e.EventId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne<EventCategory>().WithMany().HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.SetNull);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
