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
            items.ProductAmount = Math.Abs(items.ProductAmount);
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
                var log = new SimpleLogTable()
                {
                    Date = DateTime.Now,
                    UserName = username,
                    Action = "Noņemts",
                    What = items.Name,
                    Amount = items.ProductAmount,
                    Manually = "",
                    WarehouseId = catalogFromDB.WarehouseId.Value,
                    Where = catalogFromDB.Warehouse.Name
                };
                await SimpleLogTableService.AddLog(log);

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
            items.ProductAmount = Math.Abs(items.ProductAmount);
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
                var log = new SimpleLogTable()
                {
                    Date = DateTime.Now,
                    UserName = username,
                    Action = "Pievienots",
                    What = items.Name,
                    Amount = items.ProductAmount,
                    Manually = "",
                    WarehouseId = catalogFromDB.WarehouseId.Value,
                    Where = catalogFromDB.Warehouse.Name
                };
                await SimpleLogTableService.AddLog(log);

                return Ok(catalogVM);
            }
            else
            {
                return BadRequest(new { message = "Not enough products in catalog." });
            }
        }
    }
}
