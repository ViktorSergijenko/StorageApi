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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // All DB model builders are contained here(All DB Table constraints and relations)
            #region Model builders
            #region Warehouse model builder
            modelBuilder.Entity<Warehouse>()
                 .HasMany(x => x.Catalogs)
                 .WithOne(x => x.Warehouse)
                 .HasForeignKey(x => x.WarehouseId)
                 .OnDelete(DeleteBehavior.Restrict)
                 ;

            modelBuilder.Entity<Warehouse>()
                  .HasMany(x => x.WarehouseLogs)
                  .WithOne(x => x.Warehouse)
                  .HasForeignKey(x => x.WarehouseId)
                  .OnDelete(DeleteBehavior.Restrict)
                  ;

            modelBuilder.Entity<Warehouse>()
                  .HasIndex(x => new { x.Name, x.Address }).IsUnique();
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
                  .HasMany(x => x.Products)
                  .WithOne(x => x.Catalog)
                  .HasForeignKey(x => x.CatalogId)
                  ;

            modelBuilder.Entity<Catalog>()
                .HasOne(x => x.Basket)
                .WithMany(x => x.Catalogs)
                .HasForeignKey(x => x.BasketId)
                ;

            modelBuilder.Entity<Catalog>()
                .HasOne(x => x.Name)
                .WithMany(x => x.CatalogList)
                .HasForeignKey(x => x.CatalogNameId)
                ;

            modelBuilder.Entity<CatalogName>()
                .HasMany(x => x.CatalogList)
                .WithOne(x => x.Name)
                .HasForeignKey(x => x.CatalogNameId)
                .IsRequired();
                ;
            modelBuilder.Entity<CatalogName>()
                .HasOne(x => x.CatalogType)
                .WithMany(x => x.CatalogNameList)
                .HasForeignKey(x => x.CatalogTypeId)
                ;
            modelBuilder.Entity<Catalog>()
                .HasIndex(x => new { x.CatalogNameId, x.WarehouseId }).IsUnique();
           
            #endregion Catalog model builder

            #region User model builder
            modelBuilder.Entity<User>()
              .HasMany(x => x.Employees)
               .WithOne(x => x.Boss)
               .HasForeignKey(x => x.ReportsTo) // Тут я не делал IsRequired потому что у Босса нету человека которому он подчиняется это та самая рекурсивная связь
               ;
            modelBuilder.Entity<User>()
                .HasOne(x => x.Boss)
                .WithMany()
                .HasForeignKey(x => x.ReportsTo)
                ;
            modelBuilder.Entity<User>()
               .HasOne(x => x.Basket)
               .WithOne(x => x.User)
               .HasForeignKey<Basket>(x => x.UserId)
               ;
            modelBuilder.Entity<User>()
             .HasOne(x => x.Settings)
             .WithOne(x => x.User)
             .HasForeignKey<UserSettings>(x => x.UserId)
             ;
            modelBuilder.Entity<User>()
              .HasIndex(x => x.FullName)
              .IsUnique();
            #endregion User model builder

            #region Product model builder
            modelBuilder.Entity<Product>()
                .HasOne(x => x.Catalog)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CatalogId)
                .IsRequired()
                ;
            #endregion Product model builder

            #region Basket model builder
            modelBuilder.Entity<Basket>()
                .HasMany(x => x.Catalogs)
                .WithOne(x => x.Basket)
                .HasForeignKey(x => x.BasketId)
                ;
            #endregion Basket model builder

            #region News model builder
            modelBuilder.Entity<News>()
                .HasOne(x => x.Warehouse)
                .WithMany(x => x.News)
                .HasForeignKey(x => x.WarehouseId)
                ;
            modelBuilder.Entity<NewsComment>()
                .HasOne(x => x.News)
                .WithMany(x => x.NewsComments)
                .HasForeignKey(x => x.NewsId)
                ;
            #endregion News model builder

            #region UserWarehouse model builder
            modelBuilder.Entity<UserWarehouse>().HasKey(uw => new { uw.UserId, uw.WarehouseId });

            modelBuilder.Entity<UserWarehouse>()
                .HasOne<Warehouse>(sc => sc.Warehouse)
                .WithMany(s => s.UserWarehouse)
                .HasForeignKey(sc => sc.WarehouseId)
                ;

            modelBuilder.Entity<UserWarehouse>()
                .HasOne<User>(sc => sc.User)
                .WithMany(s => s.UserWarehouse)
                .HasForeignKey(sc => sc.UserId)
                ;
            #endregion UserWarehouse model builder

            #region Logs model builder
            modelBuilder.Entity<SimpleLogTable>()
                .HasOne(x => x.Warehouse)
                .WithMany(x => x.WarehouseLogs)
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict)
                ;
            #endregion Logs model builder

            #endregion Model builders

            base.OnModelCreating(modelBuilder);
        }

        // All project DB sets are contained here(DB Table descriptions like what kind of fields will be in table and other....)
        #region Db set
        public DbSet<Warehouse> WarehouseDB { get; set; }
        public DbSet<Catalog> CatalogDB { get; set; }
        public DbSet<Product> ProductDB { get; set; }
        public DbSet<News> NewsDB { get; set; }
        public DbSet<NewsComment> NewsCommentDB { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<CatalogName> CatalogNameDB { get; set; }
        public DbSet<UserWarehouse> UserWarehouseDB { get; set; }
        public DbSet<UserSettings> UserSettingsDB { get; set; }
        public DbSet<SimpleLogTable> SimpleLogTableDB { get; set; }
        public DbSet<AdminLogTable> AdminLogTable { get; set; }
        public DbSet<CatalogType> CatalogTypeDB { get; set; }
        #endregion Db set
    }
}
