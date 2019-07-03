using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class BasketController : ControllerBase
    {

        private BasketService BasketService { get; set; }
        protected StorageContext DB { get; private set; }
        private SimpleLogTableServcie SimpleLogTableService { get; set; }

        public BasketController(IServiceProvider service)
        {
            BasketService = service.GetRequiredService<BasketService>();
            DB = service.GetRequiredService<StorageContext>();
            SimpleLogTableService = service.GetRequiredService<SimpleLogTableServcie>();

        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetBasketById(Guid id)
        {
            // Getting basket by id
            var Basket = await BasketService.GetBasketById(id);
            // Returning basket 
            return Ok(Basket);
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> AddProducts([FromBody] IAddProductsToBasket items)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;

            var situation = await BasketService.AddProductsToBasket(items);
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
                    Name = catalogFromDB.Name.Name,
                    Type = catalogFromDB.Type

                };
                //await SimpleLogTableService.AddLog($"Pievienoja {items.ProductAmount} {items.Name} grozā no {catalogFromDB.Warehouse.Name} noliktava", username);

                return Ok(catalogVM);
            }
            else
            {
                return BadRequest(new { message = "Not enough products in catalog." });
            }
        }
        [HttpPost("remove-from-basket")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> RemoveProductsFromBasket([FromBody] IAddProductsToBasket items)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;

            var situation = await BasketService.AddProductsToCatalogFromBasket(items);
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
                    Name = catalogFromDB.Name.Name,
                    Type = catalogFromDB.Type

                };
                //await SimpleLogTableService.AddLog($"Noņema no groza {items.ProductAmount} {items.Name} produktus uz {catalogFromDB.Warehouse.Name} noliktavu", username);

                return Ok(catalogVM);
            }
            else
            {
                return BadRequest(new { message = "Not enough products in catalog." });
            }
        }
    }
}
