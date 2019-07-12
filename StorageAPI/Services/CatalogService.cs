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
        private SimpleLogTableServcie SimpleLogTableService { get; set; }


        public CatalogService(IServiceProvider service)
        {
            DB = service.GetService<StorageContext>();
            Mapper = service.GetRequiredService<IMapper>();
            SimpleLogTableService = service.GetRequiredService<SimpleLogTableServcie>();

        }

        #region Base crud methods
        /// <summary>
        /// Method gets catalog by id
        /// </summary>
        /// <param name="id">Id of an catalog that we want to get</param>
        /// <returns>Catalog object from DB</returns>
        public async Task<Catalog> GetCatatolById(Guid id) {
            // Getting catalog from DB
            var catalog = await DB.CatalogDB.Include(x => x.Name).FirstOrDefaultAsync(x => x.Id == id);
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
            var catalog = await DB.CatalogDB.Include(x => x.Name).Where(x => x.WarehouseId == id).ToListAsync();
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
            var catalogs = await DB.CatalogDB.Include(x => x.Name).Where(x => x.BasketId == id).ToListAsync();
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

        public async Task<List<Catalog>> GetCatatolgListByUserId(string id)
        {
            var basket = await DB.Baskets.FirstOrDefaultAsync(x => x.UserId == id);
            // Getting catalog list with a specifick warehouse id
            var catalogs = await DB.CatalogDB.Include(x => x.Name).Where(x => x.BasketId == basket.Id).ToListAsync();
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

        public async Task<Catalog> SaveCatalog(Catalog catalog, string username)
        {
            var warehouse = await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == catalog.WarehouseId);
            // If catalog does not have id, that means that it's a new entity, and we need an add functionality
            if (catalog.Id == null || catalog.Id.Equals(Guid.Empty))
            {
                var catalogName = await DB.CatalogNameDB.FirstOrDefaultAsync(x => x.Name == catalog.Name.Name);
                


                if (catalogName != null)
                {
                    catalog.CatalogNameId = catalogName.Id;
                    // Adding new warehouse to DB
                    await DB.CatalogDB.AddAsync(catalog);
                    catalogName.Amount++;
                    DB.CatalogNameDB.Update(catalogName);
                    warehouse.HasMinCatalogs = true;
                    DB.WarehouseDB.Update(warehouse);
                    //await CheckIfProductsNeedsToBeCreated(catalog);
                    await SimpleLogTableService.AddAdminLog($"Catalog {catalogName.Name} bija izveidots {warehouse.Name} noliktava", username);
                    if (catalog.CurrentAmount != 0)
                    {
                        var log = new SimpleLogTable()
                        {
                            Date = DateTime.Now,
                            UserName = username,
                            Action = "Pievienots",
                            What = catalogName.Name,
                            Amount = Math.Abs(catalog.CurrentAmount),
                            Manually = "Manuāli",
                            WarehouseId = warehouse.Id,
                            Where = warehouse.Name
                        };
                        await SimpleLogTableService.AddLog(log);
                    }
                    // Saving changes in DB
                    await DB.SaveChangesAsync();
                }
                else
                {
                    return null;
                }
            }
            // If catalog has an id, that means that it's  not a new entity, and we need an edit functionality
            else
            {
                // Getting object from DB that has similar id like in our param variable
                var catalogFromDb = await DB.CatalogDB.FirstOrDefaultAsync(x => x.Id == catalog.Id);
                // Using mapper to edit all fields
                catalogFromDb.MaximumAmount = catalog.MaximumAmount;
                catalogFromDb.MinimumAmount = catalog.MinimumAmount;
                catalogFromDb.ProductPrice = catalog.ProductPrice;
                catalogFromDb.Type = catalog.Type;
                // Updating DB
                DB.CatalogDB.Update(catalogFromDb);
                // Saving changes in DB
                await DB.SaveChangesAsync();
                await SimpleLogTableService.AddAdminLog($"Catalog {catalogFromDb.Name} informacija bija izmainīta {warehouse.Name} noliktava", username);

            }
            // Returning object
            return catalog;
        }


        /// <summary>
        /// Method deletes catalog from DB
        /// </summary>
        /// <param name="id">Id of an catalog to delete</param>
        /// <returns></returns>
        public async Task DeleteCatalog(Guid id, string username)
        {
            // Getting catalog from DB with the same id like in param
            var catalog = await DB.CatalogDB.Include(x => x.Name).Include(x => x.Warehouse).FirstOrDefaultAsync(x => x.Id == id);
            var warehouseHasCatalogs = await DB.CatalogDB.CountAsync(x => x.WarehouseId == catalog.WarehouseId);
            if (warehouseHasCatalogs >= 1)
            {
                var warehouse = await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == catalog.WarehouseId);
                warehouse.HasMinCatalogs = false;
                DB.WarehouseDB.Update(warehouse);
            }
            // Checkinf if warehouse variable for null
            if (catalog == null)
            {
                // If it's null then we throw exception
                throw new Exception("Not found");
            }
            // Removing catalog from DB
            DB.CatalogDB.Remove(catalog);
            catalog.Name.Amount--;
            DB.CatalogNameDB.Update(catalog.Name);
            // Saving changes
            await DB.SaveChangesAsync();
            await SimpleLogTableService.AddAdminLog($"Catalog {catalog.Name} bija nodzests {catalog.Warehouse.Name} noliktava", username);

        }

        public async Task<ProblemWithBasket> AddProductsToCatalogManually(IAddProductsToBasket items)
        {
            // Getting basket from Db
            var getCatalog = await GetCatatolById(items.CatalogId);
            if (getCatalog != null)
            {
                // Adding all products to catalogs in basket
                await AddProductsManually(getCatalog, items.ProductAmount);
                return ProblemWithBasket.AllIsOkey;
            }
            else
            {
                return ProblemWithBasket.NotEnoughProductsInCatalog;
            }
        }

        public async Task<ProblemWithBasket> RemoveProductsFromCatalogManually(IAddProductsToBasket items)
        {
            var getCatalog = await GetCatatolById(items.CatalogId);
            if (getCatalog.CurrentAmount >= items.ProductAmount)
            {
                //var productsToRemove = CheckForNeededAmountOfProducts(productListFromCatalog, items.ProductAmount);
                // Adding all products to catalogs in basket
                await RemoveProductsManually(items.ProductAmount, getCatalog);
                return ProblemWithBasket.AllIsOkey;
            }
            else
            {
                return ProblemWithBasket.NotEnoughProductsInCatalog;
            }
        }

        private List<Product> CheckForNeededAmountOfProducts(List<Product> products, int neededAmount)
        {
            List<Product> productsWithNeededAmount = new List<Product>();
            var amountOfProductsInList = products.Count;
            if (amountOfProductsInList < neededAmount)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < neededAmount; i++)
                {
                    productsWithNeededAmount.Add(products[i]);
                }
                return productsWithNeededAmount;
            }
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
                    productsFromCatalog[i].Name = editedCatalog.Name.Name;
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
                        Name = newCatalog.Name.Name,
                        PricePerOne = newCatalog.ProductPrice,
                        VendorCode = $"{newCatalog.Name.Name[0]}{newCatalog.Name.Name[1]}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}",
                        CatalogId = newCatalog.Id
                    };
                    await DB.ProductDB.AddAsync(newProduct);
                }
                await DB.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Method adds product in to needed catalog in basket
        /// </summary>
        /// <param name="basket">Basket in that we are putting our product</param>
        /// <param name="productList">All product list that we are putting in our basket</param>
        /// <returns>Modified basket with new products</returns>
        private async Task AddProductsManually(Catalog catalogInWarehouse, decimal amountOfProducts)
        {
            var warehouse = await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == catalogInWarehouse.WarehouseId);
            //// Looping every catalog in basket
            //Random rndNumber = new Random();
            //if (amountOfProducts > 0)
            //{ 
            //    for (int i = 0; i < amountOfProducts; i++)
            //    {
            //        Product newProduct = new Product()
            //        {
            //            Name = catalogInWarehouse.Name.Name,
            //            PricePerOne = catalogInWarehouse.ProductPrice,
            //            VendorCode = $"{catalogInWarehouse.Name.Name[0]}{catalogInWarehouse.Name.Name[1]}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}{rndNumber.Next(10)}",
            //            CatalogId = catalogInWarehouse.Id
            //        };
            //        await DB.ProductDB.AddAsync(newProduct);
            //        catalogInWarehouse.CurrentAmount++;
            //    }
            //}
            catalogInWarehouse.CurrentAmount += amountOfProducts;
            if (catalogInWarehouse.CurrentAmount <= catalogInWarehouse.MinimumAmount)
            {
                warehouse.HasMinCatalogs = true;
            }
            else
            {
                warehouse.HasMinCatalogs = false;
            }
            DB.CatalogDB.Update(catalogInWarehouse);
            // Saving all Changes in DB
            await DB.SaveChangesAsync();
        }
        /// <summary>
        /// Method adds product in to needed catalog in basket
        /// </summary>
        /// <param name="basket">Basket in that we are putting our product</param>
        /// <param name="productList">All product list that we are putting in our basket</param>
        /// <returns>Modified basket with new products</returns>
        private async Task RemoveProductsManually(decimal amountOfProducts, Catalog catalog)
        {
            var warehouse = await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == catalog.WarehouseId);
            //if (productList.Count > 0)
            //{
            //    foreach (var product in productList)
            //    {
            //        DB.ProductDB.Remove(product);
            //        catalog.CurrentAmount--;
            //    }
            //}
            catalog.CurrentAmount -= amountOfProducts;
            if (catalog.CurrentAmount <= catalog.MinimumAmount)
            {
                warehouse.HasMinCatalogs = true;
            }
            else
            {
                warehouse.HasMinCatalogs = false;
            }

            DB.CatalogDB.Update(catalog);
            // Saving all Changes in DB
            await DB.SaveChangesAsync();
        }
    }
}
