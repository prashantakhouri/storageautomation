using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wpp.StorageAutomation.Entities.Models
{
    public partial class WppsqldbContext : DbContext
    {
        public WppsqldbContext()
        {
        }

        public WppsqldbContext(DbContextOptions<WppsqldbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<Production> Production { get; set; }
        public virtual DbSet<ProductionStore> ProductionStore { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string sqlConnection = Environment.GetEnvironmentVariable("WPPSQLDBConnection");
            optionsBuilder.UseSqlServer(sqlConnection, options => options.EnableRetryOnFailure());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Groups>(entity =>
            {
                entity.Property(e => e.GroupName).HasMaxLength(100);

                entity.Property(e => e.GroupSid)
                    .HasColumnName("GroupSID")
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<Production>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ArchiveId)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ArchiveUrl).IsUnicode(false);

                entity.Property(e => e.CreatedDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");

                entity.Property(e => e.GetStatusQueryUri).IsUnicode(false);

                entity.Property(e => e.LastSyncDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ProductionStoreId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StateChangeDateTime).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Wipurl)
                    .HasColumnName("WIPUrl")
                    .IsUnicode(false);

                entity.HasOne(d => d.ProductionStore)
                    .WithMany(p => p.Production)
                    .HasForeignKey(d => d.ProductionStoreId)
                    .HasConstraintName("FK_ProdStoreId_ProdStoreDetail");
            });

            modelBuilder.Entity<ProductionStore>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ArchiveAllocatedSize).HasColumnType("decimal(38, 10)");

                entity.Property(e => e.ArchiveKeyName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('no-key-name')");

                entity.Property(e => e.ArchiveUrl)
                    .HasColumnName("ArchiveURL")
                    .IsUnicode(false);

                entity.Property(e => e.ManagerRoleGroupNames).IsUnicode(false);

                entity.Property(e => e.MinimumFreeSize).HasColumnType("decimal(38, 10)");

                entity.Property(e => e.MinimumFreeSpace).HasColumnType("decimal(38, 10)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.OfflineTime)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OnlineTime)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProductionOfflineTimeInterval).HasColumnType("decimal(38, 10)");

                entity.Property(e => e.Region)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ScaleDownTime).HasColumnType("datetime");

                entity.Property(e => e.ScaleUpTimeInterval).HasColumnType("datetime");

                entity.Property(e => e.UserRoleGroupNames).IsUnicode(false);

                entity.Property(e => e.WipallocatedSize)
                    .HasColumnName("WIPAllocatedSize")
                    .HasColumnType("decimal(38, 10)");

                entity.Property(e => e.WipkeyName)
                    .IsRequired()
                    .HasColumnName("WIPKeyName")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('no-key-name')");

                entity.Property(e => e.Wipurl)
                    .HasColumnName("WIPURL")
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
