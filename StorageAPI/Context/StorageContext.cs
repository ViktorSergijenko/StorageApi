using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StorageAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Context
{
    public class StorageContext : IdentityDbContext<User>
    {
        public StorageContext(DbContextOptions<StorageContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        #region Model builder
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Warehouse model builder
            modelBuilder.Entity<Warehouse>()
                 .HasMany(x => x.Catalogs)
                 .WithOne(x => x.Warehouse)
                 .HasForeignKey(x => x.WarehouseId)
                 .OnDelete(DeleteBehavior.Cascade)
                 ;
            #endregion Warehouse model builder

            #region Catalog model builder
            modelBuilder.Entity<Catalog>()
                .HasMany(x => x.Products)
                .WithOne(x => x.Catalog)
                .HasForeignKey(x => x.CatalogId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade)
                ;
            modelBuilder.Entity<Catalog>()
                .HasOne(x => x.Basket)
                .WithMany(x => x.Catalogs)
                .HasForeignKey(x => x.BasketId)
                ;
            #endregion Catalog model builder

            #region Product model builder
            modelBuilder.Entity<Product>()
                .HasOne(x => x.Catalog)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CatalogId)
                .IsRequired()
                ;
            #endregion Product model builder

            #region Catalog model builder
            modelBuilder.Entity<Catalog>()
                .HasMany(x => x.Products)
                .WithOne(x => x.Catalog)
                .HasForeignKey(x => x.CatalogId)
                ;

            modelBuilder.Entity<Basket>()
                .HasMany(x => x.Catalogs)
                .WithOne(x => x.Basket)
                .HasForeignKey(x => x.BasketId)
                ;

            modelBuilder.Entity<User>()
                .HasOne(x => x.Basket)
                .WithOne(x => x.User)
                .HasForeignKey<Basket>(x => x.UserId)
                ;
            #endregion Catalog model builder

            #region News model builder
            modelBuilder.Entity<News>()
                .HasOne(x => x.Warehouse)
                .WithMany(x => x.News)
                .HasForeignKey(x => x.WarehouseId)
                ;
            #endregion News model builder

            modelBuilder.Entity<CatalogName>()
            .HasMany(x => x.CatalogList)
            .WithOne(x => x.Name)
            .HasForeignKey(x => x.CatalogNameId)
            .IsRequired();
            ;

            modelBuilder.Entity<Catalog>()
            .HasOne(x => x.Name)
            .WithMany(x => x.CatalogList)
            .HasForeignKey(x => x.CatalogNameId)
            ;
            modelBuilder.Entity<User>()
              .HasOne(x => x.Settings)
              .WithOne(x => x.User)
              .HasForeignKey<UserSettings>(x => x.UserId)
              ;
            modelBuilder.Entity<UserWarehouse>().HasKey(uw => new { uw.UserId, uw.WarehouseId });

            modelBuilder.Entity<UserWarehouse>()
                .HasOne<Warehouse>(sc => sc.Warehouse)
                .WithMany(s => s.UserWarehouse)
                .HasForeignKey(sc => sc.WarehouseId);

            modelBuilder.Entity<UserWarehouse>()
                .HasOne<User>(sc => sc.User)
                .WithMany(s => s.UserWarehouse)
                .HasForeignKey(sc => sc.UserId);

            base.OnModelCreating(modelBuilder);

        }
        #endregion Model builder

        #region Db set
        public DbSet<Warehouse> WarehouseDB { get; set; }
        public DbSet<Catalog> CatalogDB { get; set; }
        public DbSet<Product> ProductDB { get; set; }
        public DbSet<News> NewsDB { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<CatalogName> CatalogNameDB { get; set; }
        public DbSet<UserWarehouse> UserWarehouseDB { get; set; }

        public DbSet<UserSettings> UserSettingsDB { get; set; }
        public DbSet<SimpleLogTable> SimpleLogTableDB { get; set; }
        public DbSet<AdminLogTable> AdminLogTable { get; set; }

        #endregion Db set
    }
}
