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
        public DbSet<Warehouse> WarehouseDB { get; set; }
    }
}
