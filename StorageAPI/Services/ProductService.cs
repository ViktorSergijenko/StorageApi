using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StorageAPI.Context;
using StorageAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Services
{
    public class ProductService
    {
        protected StorageContext DB { get; private set; }
        private readonly IMapper Mapper;

        public ProductService(IServiceProvider service)
        {
            DB = service.GetService<StorageContext>();
            Mapper = service.GetRequiredService<IMapper>();
        }
        #region BaseCrud
        /// <summary>
        /// Method gets catalog by id
        /// </summary>
        /// <param name="id">Id of an catalog that we want to get</param>
        /// <returns>Catalog object from DB</returns>
        public async Task<Product> GetProductById(Guid id)
        {
            // Getting catalog from DB
            var product = await DB.ProductDB.FirstOrDefaultAsync(x => x.Id == id);
            // Checking if it's not null
            if (product == null)
            {
                // If it's null, then we will throw new exception
                throw new Exception("Not found");
            }
            // If object was found, then we return it
            else
            {
                return product;
            }
        }

        /// <summary>
        /// Method gets products form a specifick catalog
        /// </summary>
        /// <param name="id">id of an catalog</param>
        /// <returns>List with catalog objects</returns>
        public async Task<List<Product>> GetProductByCatalogId(Guid id)
        {
            // Getting catalogs from DB
            var products = await DB.ProductDB.Where(x => x.CatalogId == id).ToListAsync();
            // Checking if it's not null
            if (products == null)
            {
                // If it's null, then we will throw new exception
                throw new Exception("Not found");
            }
            // If object was found, then we return it
            else
            {
                return products;
            }
        }

        public async Task<Product> SaveProduct(Product product)
        {
            // If catalog does not have id, that means that it's a new entity, and we need an add functionality
            if (product.Id != null)
            {
                // Adding new warehouse to DB
                DB.ProductDB.Add(product);
                // Saving changes in DB
                await DB.SaveChangesAsync();

            }
            // If catalog has an id, that means that it's  not a new entity, and we need an edit functionality
            else
            {
                // Getting object from DB that has similar id like in our param variable
                var productFromDB = await GetProductById(product.Id);
                // Using mapper to edit all fields
                product = Mapper.Map(product, productFromDB);
                var allProducts = await DB.ProductDB.Where(x => x.Name == product.Name).ToListAsync();
                allProducts.ForEach(x => {
                    x.Name = product.Name;
                    x.PricePerOne = product.PricePerOne;
                    x.VendorCode = product.VendorCode;
                    DB.ProductDB.Update(x);
                });
                // Updating DB
                DB.ProductDB.Update(product);
                // Saving changes in DB
                await DB.SaveChangesAsync();
            }
            // Returning object
            return product;
        }

        /// <summary>
        /// Method deletes product from DB
        /// </summary>
        /// <param name="id">Id of an product to delete</param>
        /// <returns></returns>
        public async Task DeleteProduct(Guid id)
        {
            // Getting product from DB with the same id like in param
            var product = await GetProductById(id);
            // Checkinf product variable for null
            if (product == null)
            {
                // If it's null then we throw exception
                throw new Exception("Not found");
            }
            // Getting catalog where this product was
            var catalogDromDb = await DB.CatalogDB.FirstOrDefaultAsync(x => x.Id == product.CatalogId);
            // Removing product from DB
            DB.ProductDB.Remove(product);
            // Decrementing current amount in catalog since we deleted product there
            catalogDromDb.CurrentAmount -= -1;
            // Saving changes
            await DB.SaveChangesAsync();
        }
        #endregion BaseCrud
    }

}
