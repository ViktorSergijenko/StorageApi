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
    public class BasketService
    {
        protected StorageContext DB { get; private set; }
        private readonly IMapper Mapper;
        public BasketService(IServiceProvider service)
        {
            DB = service.GetService<StorageContext>();
            Mapper = service.GetRequiredService<IMapper>();
        }

        /// <summary>
        /// Method gets basket by id
        /// </summary>
        /// <param name="id">Id of an basket that we want to get</param>
        /// <returns>basket object from DB</returns>
        public async Task<Basket> GetBasketById(Guid id)
        {
            // Getting catalog from DB
            var basket = await DB.Baskets.Include(x => x.Catalogs).ThenInclude(x => x.Name).FirstOrDefaultAsync(x => x.Id == id);
            // Checking if it's not null
            if (basket == null)
            {
                // If it's null, then we will throw new exception
                throw new Exception("Not found");
            }
            // If object was found, then we return it
            else
            {
                return basket;
            }
        }
        /// <summary>
        /// Method adds products to basket
        /// </summary>
        /// <param name="Id">Basket id</param>
        /// <param name="productList">Product list that we want to add to basket</param>
        /// <returns></returns>
        public async Task<ProblemWithBasket> AddProductsToBasket(IAddProductsToBasket items)
        {
            // Getting basket from Db
            var basketFromDb = await GetBasketById(items.BasketId);
            var productsFromDb = await DB.ProductDB.Where(x => x.CatalogId == items.CatalogId).ToListAsync();
            var productList = CheckForNeededAmountOfProducts(productsFromDb, items.ProductAmount);
            if (productList != null)
            {
                // Adding new catalogs to store products from list, if needed
                basketFromDb = await CheckForNewCatalogs(basketFromDb, productList);
                // Adding all products to catalogs in basket
                await AddProductsToCatalogsInBasket(basketFromDb, productList);
                return ProblemWithBasket.AllIsOkey;
            }
            else
            {
                return ProblemWithBasket.NotEnoughProductsInCatalog;
            }
          
        }
        public async Task<ProblemWithBasket> AddProductsToCatalogFromBasket(IAddProductsToBasket items)
        {
            // Getting basket from Db
            var basketFromDb = await GetBasketById(items.BasketId);
            var catalogFromBasket = basketFromDb.Catalogs.FirstOrDefault(x => x.Name.Name == items.Name);
            var catalogInWarehouse = await DB.CatalogDB.FirstOrDefaultAsync(x => x.Id == items.CatalogId);
            var productsFromDb = await DB.ProductDB.Where(x => x.CatalogId == catalogFromBasket.Id).ToListAsync();
            var productList = CheckForNeededAmountOfProducts(productsFromDb, items.ProductAmount);
            if (productsFromDb != null)
            {
                // Adding all products to catalogs in basket
                await AddProductsToCatalogsFromBasket(basketFromDb, catalogFromBasket, catalogInWarehouse, productList);
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
        /// <summary>
        /// Method checks does our basket has needed catalogs to store products that has came with product list in param
        /// </summary>
        /// <param name="basket">Basket in that we are going to put all products from productList</param>
        /// <param name="productList">All products that we are going to put in basket</param>
        /// <returns>New basket with new catalogs</returns>
        private async Task<Basket> CheckForNewCatalogs(Basket basket, List<Product> productList)
        {
            // Looping all products from product list
            foreach (var product in productList)
            {
                // If there's no catalog with the same name as product, that means that we need to add new catalog to basket, so we could store this product
                if (!basket.Catalogs.Any(x => x.Name.Name == product.Name))
                {
                    // Creating new catalog
                    Catalog newCatalog = new Catalog();
                    var catalogName = await DB.CatalogNameDB.FirstOrDefaultAsync(x => x.Name == product.Name);
                    newCatalog.CatalogNameId = catalogName.Id;
                    newCatalog.BasketId = basket.Id;
                    // Adding it to DB
                    await DB.CatalogDB.AddAsync(newCatalog);
                }
            }
            // Saving all changes in DB
            await DB.SaveChangesAsync();
            // Returning new basket
            return basket;
        }

        /// <summary>
        /// Method adds product in to needed catalog in basket
        /// </summary>
        /// <param name="basket">Basket in that we are putting our product</param>
        /// <param name="productList">All product list that we are putting in our basket</param>
        /// <returns>Modified basket with new products</returns>
        private async Task AddProductsToCatalogsInBasket(Basket basket, List<Product> productList)
        {
            var catalogToEdit = await DB.CatalogDB.FirstOrDefaultAsync(x => x.Id ==productList[0].CatalogId);
            // Looping every catalog in basket
            var catalog = basket.Catalogs.FirstOrDefault(x => x.Name == catalogToEdit.Name);
                // Second loop goest thru all products in products list
                foreach (var products in productList)
                {
                    // We check every catalog name with the name of product, since all products has the same name as catalog where they are stored in
                    // If name of catalog and product name is the same, that means that we need to put this product in this catalog
                    if (catalog.Name.Name == products.Name)
                    {
                        // When we found product with the same name as catalog, we putt this item in this catalog
                        // We are creating new product with the same params, except catalog id, because we take catalog from basket
                        Product newProduct = new Product()
                        {
                            Name = products.Name,
                            VendorCode = products.VendorCode,
                            PricePerOne = products.PricePerOne,
                            // Setting catalog id to the catalog that is in the basket
                            CatalogId = catalog.Id
                        };
                        catalog.CurrentAmount++;
                        // Removing product from DB
                        DB.ProductDB.Remove(products);
                        DB.CatalogDB.Update(catalog);

                        // And creating it in the new catalog in basket
                        await DB.ProductDB.AddAsync(newProduct);
                    }
                    
                }
                catalogToEdit.CurrentAmount = catalogToEdit.CurrentAmount - productList.Count;
                DB.CatalogDB.Update(catalogToEdit);
            
            // Saving all Changes in DB
            await DB.SaveChangesAsync();
        }
        /// <summary>
        /// Method adds product in to needed catalog in basket
        /// </summary>
        /// <param name="basket">Basket in that we are putting our product</param>
        /// <param name="productList">All product list that we are putting in our basket</param>
        /// <returns>Modified basket with new products</returns>
        private async Task AddProductsToCatalogsFromBasket(Basket basket, Catalog catalogFromBasket, Catalog catalogInWarehouse, List<Product> productList)
        {
            // Looping every catalog in basket
            foreach (var product in productList)
            {
               
                        // When we found product with the same name as catalog, we putt this item in this catalog
                        // We are creating new product with the same params, except catalog id, because we take catalog from basket
                        Product newProduct = new Product()
                        {
                            Name = product.Name,
                            VendorCode = product.VendorCode,
                            PricePerOne = product.PricePerOne,
                            // Setting catalog id to the catalog that is in the basket
                            CatalogId = catalogInWarehouse.Id
                        };
                        catalogInWarehouse.CurrentAmount++;
                        DB.CatalogDB.Update(catalogInWarehouse);
                        // Removing product from DB
                        DB.ProductDB.Remove(product);
                        catalogFromBasket.CurrentAmount--;
                        if (catalogFromBasket.CurrentAmount == 0)
                        {
                            DB.CatalogDB.Remove(catalogFromBasket);
                        }
                        else
                        {
                        DB.CatalogDB.Update(catalogFromBasket);

                        }

                        // And creating it in the new catalog in basket
                        await DB.ProductDB.AddAsync(newProduct);
            }
            // Saving all Changes in DB
            await DB.SaveChangesAsync();
        }

    }
}
