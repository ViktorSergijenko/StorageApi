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

        public async Task<List<Catalog>> GetCatatolgListByWarehouseId(Guid id)
        {
            // Getting catalog list with a specifick warehouse id
            var catalog = await DB.CatalogDB.Where(x => x.WarehouseId == id).ToListAsync();
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

        public async Task<List<Catalog>> GetCatatolgListByBasketeId(Guid id)
        {
            // Getting catalog list with a specifick warehouse id
            var catalogs = await DB.CatalogDB.Where(x => x.BasketId == id).ToListAsync();
            // Checking if it's not null
            if (catalogs == null)
            {
                // If it's null, then we will throw new exception
                throw new Exception("Not found");
            }
            // If object was found, then we return it
            else
            {
                return catalogs;
            }
        }

        public async Task<Catalog> SaveCatalog(Catalog catalog)
        {
            // If catalog does not have id, that means that it's a new entity, and we need an add functionality
            if (catalog.Id == null || catalog.Id.Equals(Guid.Empty))
            {
                // Adding new warehouse to DB
                await DB.CatalogDB.AddAsync(catalog);
                await CheckIfProductsNeedsToBeCreated(catalog);
                // Saving changes in DB
                await DB.SaveChangesAsync();

            }
            // If catalog has an id, that means that it's  not a new entity, and we need an edit functionality
            else
            {
                // Getting object from DB that has similar id like in our param variable
                var catalogFromDb = await DB.CatalogDB.FirstOrDefaultAsync(x => x.Id == catalog.Id);
                // Checking if name was changed or not
                await CheckIfCatalogNameWasChanged(catalog, catalogFromDb);
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

        /// <summary>
        /// Method checks if catalog name was changed, since all products that are stored in catalog has the same name as catalog, we need to change product names too
        /// </summary>
        /// <param name="editedCatalog">Edited catalog</param>
        /// <param name="catalogFromDb">Catalog with old values</param>
        /// <returns></returns>
        private async Task CheckIfCatalogNameWasChanged(Catalog editedCatalog, Catalog catalogFromDb)
        {
            // Checking if name is the same or not
            if (editedCatalog.Name != catalogFromDb.Name)
            {
                // If it's new, then we get all catalog products
                var productsFromCatalog = await DB.ProductDB.Where(x => x.CatalogId == editedCatalog.Id).ToListAsync();
                // And changing each product name and saving that
                //productsFromCatalog.ForEach(x => {
                //    x.Name = editedCatalog.Name;
                //    DB.ProductDB.Update(x);
                //});
                for (int i = 0; i < productsFromCatalog.Count; i++)
                {
                    productsFromCatalog[i].Name = editedCatalog.Name;
                    DB.ProductDB.Update(productsFromCatalog[i]);
                }
                // Saving changes in DB
                await DB.SaveChangesAsync();
            }
        }

        private async Task CheckIfProductsNeedsToBeCreated(Catalog newCatalog)
        {
            Random rndNumber = new Random();
            if (newCatalog.CurrentAmount > 0)
            {
                for (int i = 0; i < newCatalog.CurrentAmount; i++)
                {
                    Product newProduct = new Product()
                    {
                        Name = newCatalog.Name,
                        PricePerOne = newCatalog.ProductPrice,
                        VendorCode = $"{newCatalog.Name[0]}{newCatalog.Name[1]}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}",
                        CatalogId = newCatalog.Id
                    };
                    await DB.ProductDB.AddAsync(newProduct);
                }
                await DB.SaveChangesAsync();
            }
        }
    }
}
