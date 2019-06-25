using AutoMapper;
using StorageAPI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StorageAPI.Models;

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
        public async Task<List<CatalogName>> GetCatalogNameList()
        {
            var catalogList = await DB.CatalogNameDB.ToListAsync();
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
                await SimpleLogTableService.AddAdminLog($"Created catalog name: {catalogName.Name}", username);

            }
            else
            {
                var catalogNameFromDb = await DB.CatalogNameDB.FirstOrDefaultAsync(x => x.Id == catalogName.Id);
                var oldCatalogName = catalogNameFromDb.Name;
                catalogNameFromDb.Name = catalogName.Name;
                DB.CatalogNameDB.Update(catalogNameFromDb);
                await SimpleLogTableService.AddAdminLog($"Changed catalog name from '{oldCatalogName}' to '{catalogName.Name}'", username);
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
                catalogNameQuery = catalogNameQuery.Where(x => x.Name.ToLower().Contains(filter)).ToList();
            }
            else
            {
                // TODO: This is only for short period of time, need to make functionality where user can chose by what field user can sort and in whick direction
                // If our option is null, then we just sorting our query by name
                catalogNameQuery = catalogNameQuery.OrderByDescending(x => x.Name).ToList();
            }
            return catalogNameQuery;
        }

    }
}
