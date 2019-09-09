using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StorageAPI.Context;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOneOrigin")]
    [ApiController]
    public class HelpController : ControllerBase
    {
        protected StorageContext DB { get; private set; }
        public HelpController(IServiceProvider service)
        {
            DB = service.GetRequiredService<StorageContext>();
        }
        //// GET: api/Help
        //[HttpGet("fix-catalog-types")]
        //public async Task<string> FixCatalogType()
        //{
        //    var catalogNames = await DB.CatalogDB.Include(x => x.Name).ToListAsync();
        //    foreach (var catalog in catalogNames)
        //    {
        //        if (catalog.Type == false)
        //        {
        //            catalog.Name.UserId = "de0717aa-e628-49b0-bb5d-a43331c0ab67";
        //            var snekType = await DB.CatalogTypeDB.FirstOrDefaultAsync(x => x.Name == "Sneks");
        //            catalog.Name.CatalogTypeId = snekType.Id;
        //            DB.CatalogNameDB.Update(catalog.Name);
        //        }
        //        else
        //        {
        //            catalog.Name.UserId = "de0717aa-e628-49b0-bb5d-a43331c0ab67";
        //            var snekType = await DB.CatalogTypeDB.FirstOrDefaultAsync(x => x.Name == "Kafija");
        //            catalog.Name.CatalogTypeId = snekType.Id;
        //            DB.CatalogNameDB.Update(catalog.Name);
        //        }
        //    }
        //   await DB.SaveChangesAsync();
        //   return "types fixed";
        //}
        [HttpGet("fix-catalog-types-amount")]
        public async Task<string> FixTypeAmount()
        {
            var catalogNames = await DB.CatalogTypeDB.Include(x => x.CatalogNameList).ToListAsync();
            foreach (var catalog in catalogNames)
            {
                catalog.Amount = catalog.CatalogNameList.Count;
                DB.CatalogTypeDB.Update(catalog);
            }
            await DB.SaveChangesAsync();
            return "types fixed";
        }

        [HttpGet("fix-house-problem")]
        public async Task<string> FixHouseProblem()
        {
            var houses = await DB.WarehouseDB.Include(x => x.News).ToListAsync();
            foreach (var house in houses)
            {
                var hasProblems = await DB.WarehouseDB.AnyAsync(x => x.News.Any(o => o.FixedProblem != true && o.WarehouseId == house.Id));
                if (hasProblems)
                {
                    // If has, then we get this house and change it flag hasProblems to true
                    var houseWithProblem = await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == house.Id);
                    house.HasProblems = true;
                    DB.WarehouseDB.Update(houseWithProblem);

                }
                else
                {
                    // If that was the last news that has not been resolved, then we change has problems flag to false
                    var houseWithNoProblems = await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == house.Id);
                    house.HasProblems = false;
                    DB.WarehouseDB.Update(house);
                }
            }
            await DB.SaveChangesAsync();
            return "hosue problems fixed";
        }

        [HttpGet("fix-min-problem-catalog")]
        public async Task<string> FixCatalogs()
        {
            var catalogswITHmIN = await DB.CatalogDB.Include(x => x.Warehouse).Where(x => x.WarehouseId != null && x.WarehouseId != Guid.Empty && x.CurrentAmount < x.MinimumAmount).ToListAsync();
            var catalogsWithoutMiN = await DB.CatalogDB.Include(x => x.Warehouse).Where(x => x.WarehouseId != null && x.WarehouseId != Guid.Empty && x.CurrentAmount >= x.MinimumAmount).ToListAsync();
            foreach (var catalog in catalogswITHmIN)
            {
                catalog.Warehouse.HasMinCatalogs = true;
                DB.WarehouseDB.Update(catalog.Warehouse);
            }
            foreach (var catalog in catalogsWithoutMiN)
            {
                catalog.Warehouse.HasMinCatalogs = false;
                DB.WarehouseDB.Update(catalog.Warehouse);
            }
            await DB.SaveChangesAsync();
            return "catalog problems fixed";
        }

    }
}
