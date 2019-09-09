using AutoMapper;
using StorageAPI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StorageAPI.Models;
using StorageAPI.ModelsVM;

namespace StorageAPI.Services
{
    public class CatalogNameService
    {
        protected StorageContext DB { get; private set; }
        private readonly IMapper Mapper;
        private SimpleLogTableServcie SimpleLogTableService { get; set; }

        public CatalogNameService(IServiceProvider service)
        {
            DB = service.GetService<StorageContext>();
            Mapper = service.GetRequiredService<IMapper>();
            SimpleLogTableService = service.GetRequiredService<SimpleLogTableServcie>();

        }

        /// <summary>
        /// Method gets all Catalog names from DB
        /// </summary>
        /// <returns></returns>
        public async Task<List<CatalogName>> GetCatalogNameList(Guid id)
        {
            var catalogList = await DB.CatalogNameDB.Where(x => x.CatalogTypeId == id).ToListAsync();
            return catalogList;
        }

        /// <summary>
        /// Method gets catalog name from DB by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CatalogName> GetCatalogNameById(Guid id)
        {
            var catalogName = await DB.CatalogNameDB.FirstOrDefaultAsync(x => x.Id == id);
            return catalogName;
        }

        public async Task<CatalogName> SaveCataogName(CatalogName catalogName, string username)
        {
            if (catalogName.Id == null || catalogName.Id.Equals(Guid.Empty))
            {
                await DB.CatalogNameDB.AddAsync(catalogName);
                var catalogType = await DB.CatalogTypeDB.FirstOrDefaultAsync(x => x.Id == catalogName.CatalogTypeId);
                catalogType.Amount++;
                DB.CatalogTypeDB.Update(catalogType);
                await SimpleLogTableService.AddAdminLog($"Izveidoja kataloga nosaukumu: {catalogName.Name}", username);

            }
            else
            {
                var catalogNameFromDb = await DB.CatalogNameDB.FirstOrDefaultAsync(x => x.Id == catalogName.Id);
                var oldCatalogName = catalogNameFromDb.Name;
                catalogNameFromDb.Name = catalogName.Name;
                DB.CatalogNameDB.Update(catalogNameFromDb);
                await SimpleLogTableService.AddAdminLog($"Mainīja kataloga nosaukumu no '{oldCatalogName}' uz '{catalogName.Name}'", username);
            }
            await DB.SaveChangesAsync();
            return catalogName;
        }

        public async Task<List<CatalogName>> FilterCatalogName(FilterSorting filterSorting)
        {
            // Getting our catalog name query, that we will filter
            var catalogNameQuery = await DB.CatalogNameDB.ToListAsync();
            // Checking if our filter option is null
            if (!String.IsNullOrEmpty(filterSorting.FilterOption))
            {
                // If it's not null, then we set this option to lover case
                var filter = filterSorting.FilterOption.ToLower();
                // Filtering our query, where warehouse address or name contains something similar to our option
                catalogNameQuery = catalogNameQuery.Where(x => x.Name.ToLower().Contains(filter) && x.CatalogTypeId == filterSorting.CatalogTypeId).ToList();
            }
            else
            {
                // TODO: This is only for short period of time, need to make functionality where user can chose by what field user can sort and in whick direction
                // If our option is null, then we just sorting our query by name
                catalogNameQuery = catalogNameQuery.Where(x => x.CatalogTypeId == filterSorting.CatalogTypeId)
                    .OrderByDescending(x => x.Name)
                    .ToList();
            }
            return catalogNameQuery;
        }

        /// <summary>
        /// Method gets all warehouses that stores catalogs with specific name
        /// </summary>
        /// <param name="catalogNameId">Catalog name id</param>
        /// <returns>Warehouse list</returns>
        public async Task<List<WarehouseVM>> GetWarehousesThatHasCatalogsWithSpecificName(Guid catalogNameId)
        {
            // Getting and returning all warehouses that stores catalogs with specific name
            return await DB.WarehouseDB.Where(x => x.Catalogs.Any(y => y.CatalogNameId == catalogNameId))
                .Select(x=> new WarehouseVM()
                {
                    Id = x.Id,
                    Address = x.Address,
                    Name = x.Name
                })
                .ToListAsync();         
        }
    }
}
