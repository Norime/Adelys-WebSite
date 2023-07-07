using Adelys_WebSite.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Adelys_WebSite.DAL
{
    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext() : base()
        {
        }

        public MySqlDbContext(DbContextOptions<MySqlDbContext> options)
        : base(options)
        {
        }

        public virtual DbSet<LuckpermsAction> LuckpermsActions { get; set; }

        public virtual DbSet<LuckpermsGroup> LuckpermsGroups { get; set; }

        public virtual DbSet<LuckpermsGroupPermission> LuckpermsGroupPermissions { get; set; }

        public virtual DbSet<LuckpermsMessenger> LuckpermsMessengers { get; set; }

        public virtual DbSet<LuckpermsPlayer> LuckpermsPlayers { get; set; }

        public virtual DbSet<LuckpermsTrack> LuckpermsTracks { get; set; }

        public virtual DbSet<LuckpermsUserPermission> LuckpermsUserPermissions { get; set; }

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        //        => optionsBuilder.UseMySql("server=sql3.minestrator.com;database=minesr_sXvnn1Ht;uid=minesr_sXvnn1Ht;pwd=eJQVoFuveaJdxQpy", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.27-mysql"));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<LuckpermsAction>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("luckperms_actions");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ActedName)
                    .HasMaxLength(36)
                    .HasColumnName("acted_name");
                entity.Property(e => e.ActedUuid)
                    .HasMaxLength(36)
                    .HasColumnName("acted_uuid");
                entity.Property(e => e.Action)
                    .HasMaxLength(300)
                    .HasColumnName("action");
                entity.Property(e => e.ActorName)
                    .HasMaxLength(100)
                    .HasColumnName("actor_name");
                entity.Property(e => e.ActorUuid)
                    .HasMaxLength(36)
                    .HasColumnName("actor_uuid");
                entity.Property(e => e.Time).HasColumnName("time");
                entity.Property(e => e.Type)
                    .HasMaxLength(1)
                    .IsFixedLength()
                    .HasColumnName("type");
            });

            modelBuilder.Entity<LuckpermsGroup>(entity =>
            {
                entity.HasKey(e => e.Name).HasName("PRIMARY");

                entity.ToTable("luckperms_groups");

                entity.Property(e => e.Name)
                    .HasMaxLength(36)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<LuckpermsGroupPermission>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("luckperms_group_permissions");

                entity.HasIndex(e => e.Name, "luckperms_group_permissions_name");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Contexts)
                    .HasMaxLength(200)
                    .HasColumnName("contexts");
                entity.Property(e => e.Expiry).HasColumnName("expiry");
                entity.Property(e => e.Name)
                    .HasMaxLength(36)
                    .HasColumnName("name");
                entity.Property(e => e.Permission)
                    .HasMaxLength(200)
                    .HasColumnName("permission");
                entity.Property(e => e.Server)
                    .HasMaxLength(36)
                    .HasColumnName("server");
                entity.Property(e => e.Value).HasColumnName("value");
                entity.Property(e => e.World)
                    .HasMaxLength(64)
                    .HasColumnName("world");
            });

            modelBuilder.Entity<LuckpermsMessenger>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("luckperms_messenger");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Msg)
                    .HasColumnType("text")
                    .HasColumnName("msg");
                entity.Property(e => e.Time)
                    .HasColumnType("timestamp")
                    .HasColumnName("time");
            });

            modelBuilder.Entity<LuckpermsPlayer>(entity =>
            {
                entity.HasKey(e => e.Uuid).HasName("PRIMARY");

                entity.ToTable("luckperms_players");

                entity.HasIndex(e => e.Username, "luckperms_players_username");

                entity.Property(e => e.Uuid)
                    .HasMaxLength(36)
                    .HasColumnName("uuid");
                entity.Property(e => e.PrimaryGroup)
                    .HasMaxLength(36)
                    .HasColumnName("primary_group");
                entity.Property(e => e.Username)
                    .HasMaxLength(16)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<LuckpermsTrack>(entity =>
            {
                entity.HasKey(e => e.Name).HasName("PRIMARY");

                entity.ToTable("luckperms_tracks");

                entity.Property(e => e.Name)
                    .HasMaxLength(36)
                    .HasColumnName("name");
                entity.Property(e => e.Groups)
                    .HasColumnType("text")
                    .HasColumnName("groups");
            });

            modelBuilder.Entity<LuckpermsUserPermission>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("luckperms_user_permissions");

                entity.HasIndex(e => e.Uuid, "luckperms_user_permissions_uuid");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Contexts)
                    .HasMaxLength(200)
                    .HasColumnName("contexts");
                entity.Property(e => e.Expiry).HasColumnName("expiry");
                entity.Property(e => e.Permission)
                    .HasMaxLength(200)
                    .HasColumnName("permission");
                entity.Property(e => e.Server)
                    .HasMaxLength(36)
                    .HasColumnName("server");
                entity.Property(e => e.Uuid)
                    .HasMaxLength(36)
                    .HasColumnName("uuid");
                entity.Property(e => e.Value).HasColumnName("value");
                entity.Property(e => e.World)
                    .HasMaxLength(64)
                    .HasColumnName("world");
            });

            base.OnModelCreating(modelBuilder);
        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
