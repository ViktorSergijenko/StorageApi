﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StorageAPI.Context;
using StorageAPI.Models;
using StorageAPI.ModelsVM;
using StorageAPI.Services;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOneOrigin")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private CatalogService CatalogService { get; set; }
        protected StorageContext DB { get; private set; }
        private SimpleLogTableServcie SimpleLogTableService { get; set; }

        public CatalogController(IServiceProvider service)
        {
            CatalogService = service.GetRequiredService<CatalogService>();
            DB = service.GetRequiredService<StorageContext>();
            SimpleLogTableService = service.GetRequiredService<SimpleLogTableServcie>();

        }


        /// <summary>
        /// Method gets all catalogs from DB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Catalog>>> GetAllCatalogs()
        {
           var catalogs =  await DB.CatalogDB.Include(x => x.Name).ToListAsync();
            return Ok(Mapper.Map<List<CatalogVM>>(catalogs));
        }

        /// <summary>
        /// Method gets Catalog by id
        /// </summary>
        /// <param name="id">Id of a Catalog that we want to get</param>
        /// <returns>Ok status with an Catalog object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCatalogById(Guid id)
        {
            // Getting Catalog by id
            var Catalog = await CatalogService.GetCatatolById(id);
            // Returning Catalog 
            return Ok(Catalog);
        }

        /// <summary>
        /// Method gets Catalog by id
        /// </summary>
        /// <param name="id">Id of a Catalog that we want to get</param>
        /// <returns>Ok status with an Catalog object</returns>
        [HttpGet("warehouse/{id}")]
        public async Task<ActionResult> GetCatalogByWarehouseId(Guid id)
        {
            // Getting Catalog by id
            var catalogsList = await CatalogService.GetCatatolgListByWarehouseId(id);
            List<CatalogVM> catalogListVM = new List<CatalogVM>();
            catalogsList.ForEach(x => {
                var catalogVM = new CatalogVM
                {
                    Id = x.Id,
                    ProductPrice = x.ProductPrice,
                    CurrentAmount = x.CurrentAmount,
                    MaximumAmount = x.MaximumAmount,
                    MinimumAmount = x.MinimumAmount,
                    WarehouseId = x.WarehouseId,
                    Name = x.Name.Name               
                };

                catalogListVM.Add(catalogVM);
            });
            // Returning Catalog 
            return Ok(catalogListVM);
        }
        /// <summary>
        /// Method gets Catalog by id
        /// </summary>
        /// <param name="id">Id of a Catalog that we want to get</param>
        /// <returns>Ok status with an Catalog object</returns>
        [HttpGet("basket/{id}")]
        public async Task<ActionResult> GetCatalogByBasketId(Guid id)
        {
            // Getting Catalog by basket id
            var Catalog = await CatalogService.GetCatatolgListByBasketeId(id);
            List<CatalogVM> catalogListVM = new List<CatalogVM>();
            Catalog.ForEach(x => {
                var catalogVM = new CatalogVM
                {
                    Id = x.Id,
                    ProductPrice = x.ProductPrice,
                    CurrentAmount = x.CurrentAmount,
                    MaximumAmount = x.MaximumAmount,
                    MinimumAmount = x.MinimumAmount,
                    WarehouseId = x.WarehouseId,
                    Name = x.Name.Name
                };

                catalogListVM.Add(catalogVM);
            });
            // Returning Catalog 
            return Ok(catalogListVM);
        }

        [HttpGet("basket-user-id/{id}")]
        public async Task<ActionResult> GetCatalogByUserId(string id)
        {
            // Getting Catalog by basket id
            var Catalog = await CatalogService.GetCatatolgListByUserId(id);
            List<CatalogVM> catalogListVM = new List<CatalogVM>();
            Catalog.ForEach(x => {
                var catalogVM = new CatalogVM
                {
                    Id = x.Id,
                    ProductPrice = x.ProductPrice,
                    CurrentAmount = x.CurrentAmount,
                    MaximumAmount = x.MaximumAmount,
                    MinimumAmount = x.MinimumAmount,
                    WarehouseId = x.WarehouseId,
                    Name = x.Name.Name
                };

                catalogListVM.Add(catalogVM);
            });
            // Returning Catalog 
            return Ok(catalogListVM);
        }

        /// <summary>
        /// Method adds or modifies a  Catalog in DB
        /// </summary>
        /// <param name="Catalog">Catalog object that we want to add or edit</param>
        /// <returns> Catalog object</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> CreateCatalog([FromBody] CatalogVM Catalog)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
            var catalog = new Catalog()
            {
                Name = new CatalogName()
                {
                    Name = Catalog.Name
                },
                Id = Catalog.Id,
                ProductPrice = Catalog.ProductPrice,
                CurrentAmount = Catalog.CurrentAmount,
                MaximumAmount = Catalog.MaximumAmount,
                MinimumAmount = Catalog.MinimumAmount,
                WarehouseId = Catalog.WarehouseId 
            };
            // Adding new Catalog by calling a method that will add it to DB
            var newCatalog = await CatalogService.SaveCatalog(catalog, username);
            if (newCatalog == null)
            {
                return BadRequest(new { message = "There is no such catalog name." });
            }

            var catalogVM = new CatalogVM
            {
                Id = newCatalog.Id,
                ProductPrice = newCatalog.ProductPrice,
                CurrentAmount = newCatalog.CurrentAmount,
                MaximumAmount = newCatalog.MaximumAmount,
                MinimumAmount = newCatalog.MinimumAmount,
                WarehouseId = newCatalog.WarehouseId,
                Name = newCatalog.Name.Name
            };
            // Returning new Catalog
            return Ok(catalogVM);
        }

        /// <summary>
        /// Method deletes Catalog from DB
        /// </summary>
        /// <param name="id">Id of an Catalog that we want to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> FilterCatalogs(Guid id)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;

            // Calling method that will delete Catalog from DB
            await CatalogService.DeleteCatalog(id, username);
            // Returning filtered Catalog list
            return Ok();
        }
        [HttpPost("addProductsManually")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> AddProductsManually([FromBody] IAddProductsToBasket items)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;

            var situation = await CatalogService.AddProductsToCatalogManually(items);
            if (situation == ProblemWithBasket.AllIsOkey)
            {
                var catalogFromDB = await DB.CatalogDB.Include(x => x.Warehouse).FirstOrDefaultAsync(x => x.Id == items.CatalogId);
                 var catalogVM = new CatalogVM
                {
                    Id = catalogFromDB.Id,
                    ProductPrice = catalogFromDB.ProductPrice,
                    CurrentAmount = catalogFromDB.CurrentAmount,
                    MaximumAmount = catalogFromDB.MaximumAmount,
                    MinimumAmount = catalogFromDB.MinimumAmount,
                    WarehouseId = catalogFromDB.WarehouseId,
                    Name = catalogFromDB.Name.Name
                };
                await SimpleLogTableService.AddLog($"Added manually {items.ProductAmount} {items.Name} products to {catalogVM.Name} in {catalogFromDB.Warehouse.Name} warehouse", username);
                return Ok(catalogVM);
            }
            else
            {
                return BadRequest(new { message = "Something went wrong." });
            }
        }
        [HttpPost("removeProductsManually")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> RemoveProductsManually([FromBody] IAddProductsToBasket items)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
            var situation = await CatalogService.RemoveProductsFromCatalogManually(items);
            if (situation == ProblemWithBasket.AllIsOkey)
            {
                var catalogFromDB = await DB.CatalogDB.Include(x => x.Warehouse).FirstOrDefaultAsync(x => x.Id == items.CatalogId);
                var catalogVM = new CatalogVM
                {
                    Id = catalogFromDB.Id,
                    ProductPrice = catalogFromDB.ProductPrice,
                    CurrentAmount = catalogFromDB.CurrentAmount,
                    MaximumAmount = catalogFromDB.MaximumAmount,
                    MinimumAmount = catalogFromDB.MinimumAmount,
                    WarehouseId = catalogFromDB.WarehouseId,
                    Name = catalogFromDB.Name.Name
                };
                await SimpleLogTableService.AddLog($"Removed manually {items.ProductAmount} {items.Name} products from {catalogVM.Name} in {catalogFromDB.Warehouse.Name} warehouse", username);

                return Ok(catalogVM);
            }
            else
            {
                return BadRequest(new { message = "Not enough products in catalog." });
            }
        }
    }

    ///// <summary>
    ///// Method filters Catalogs by filter option
    ///// </summary>
    ///// <param name="option">Filter option, by which filtration will be done</param>
    ///// <returns>Filtered Catalog list</returns>
    //[HttpPost("filteredCatalog")]
    //public async Task<ActionResult> FilterCatalogs([FromBody] FilterSorting filterSorting)
    //{

    //    // Filtering Catalog query by calling method that will filter it
    //    var newCatalog = await CatalogService.FilterCatalogs(filterSorting);
    //    // Returning filtered Catalog list
    //    return Ok(newCatalog);
    //}
}

