using Microsoft.EntityFrameworkCore;
using StorageAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Context
{
    public class StorageContext : DbContext
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
                 .IsRequired()
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
            #endregion Catalog model builder

            base.OnModelCreating(modelBuilder);
        }
        #endregion Model builder

        #region Db set
        public DbSet<Warehouse> WarehouseDB { get; set; }
        public DbSet<Catalog> CatalogDB { get; set; }
        public DbSet<Product> ProductDB { get; set; }
        #endregion Db set
    }
}
