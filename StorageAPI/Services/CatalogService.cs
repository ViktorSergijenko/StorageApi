using StorageAPI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StorageAPI.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace StorageAPI.Services
{
    public class CatalogService
    {
        protected StorageContext DB { get; private set; }
        private readonly IMapper Mapper;

        public CatalogService(IServiceProvider service)
        {
            DB = service.GetService<StorageContext>();
            Mapper = service.GetRequiredService<IMapper>();
        }

        #region Base crud methods
        /// <summary>
        /// Method gets catalog by id
        /// </summary>
        /// <param name="id">Id of an catalog that we want to get</param>
        /// <returns>Catalog object from DB</returns>
        public async Task<Catalog> GetCatatolById(Guid id) {
            // Getting catalog from DB
            var catalog = await DB.CatalogDB.FirstOrDefaultAsync(x => x.Id == id);
            // Checking if it's not null
            if (catalog == null)
            {
                // If it's null, then we will throw new exception
                throw new Exception("Not found");
            }
            // If object was found, then we return it
            else
            {
                return catalog;
            }
        }

        public async Task<Catalog> SaveCatalog(Catalog catalog)
        {
            // If catalog does not have id, that means that it's a new entity, and we need an add functionality
            if (catalog.Id != null)
            {
                // Adding new warehouse to DB
                DB.CatalogDB.Add(catalog);
                // Saving changes in DB
                await DB.SaveChangesAsync();

            }
            // If catalog has an id, that means that it's  not a new entity, and we need an edit functionality
            else
            {
                // Getting object from DB that has similar id like in our param variable
                var catalogFromDb = await DB.CatalogDB.FirstOrDefaultAsync(x => x.Id == catalog.Id);
                // Using mapper to edit all fields
                catalog = Mapper.Map(catalog, catalogFromDb);
                // Updating DB
                DB.CatalogDB.Update(catalog);
                // Saving changes in DB
                await DB.SaveChangesAsync();
            }
            // Returning object
            return catalog;
        }

        /// <summary>
        /// Method deletes catalog from DB
        /// </summary>
        /// <param name="id">Id of an catalog to delete</param>
        /// <returns></returns>
        public async Task DeleteCatalog(Guid id)
        {
            // Getting catalog from DB with the same id like in param
            var catalog = await DB.CatalogDB.FirstOrDefaultAsync(x => x.Id == id);
            // Checkinf if warehouse variable for null
            if (catalog == null)
            {
                // If it's null then we throw exception
                throw new Exception("Not found");
            }
            // Removing catalog from DB
            DB.CatalogDB.Remove(catalog);
            // Saving changes
            await DB.SaveChangesAsync();
        }
        #endregion Base crud methods
    }
}
