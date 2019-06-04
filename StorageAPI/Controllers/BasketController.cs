using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StorageAPI.Context;
using StorageAPI.Models;
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
        public BasketController(IServiceProvider service)
        {
            BasketService = service.GetRequiredService<BasketService>();
            DB = service.GetRequiredService<StorageContext>();
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
        public async Task<ActionResult> AddProducts([FromBody] IAddProductsToBasket items)
        {
            var situation = await BasketService.AddProductsToBasket(items);
            if (situation == ProblemWithBasket.AllIsOkey)
            {
                
                return Ok(await DB.CatalogDB.FirstOrDefaultAsync(x => x.Id == items.CatalogId));
            }
            else
            {
                return BadRequest(new { message = "Not enough products in catalog." });
            }
        }
        [HttpPost("remove-from-basket")]
        public async Task<ActionResult> RemoveProductsFromBasket([FromBody] IAddProductsToBasket items)
        {
            var situation = await BasketService.AddProductsToCatalogFromBasket(items);
            if (situation == ProblemWithBasket.AllIsOkey)
            {

                return Ok(await DB.CatalogDB.FirstOrDefaultAsync(x => x.Id == items.CatalogId));
            }
            else
            {
                return BadRequest(new { message = "Not enough products in catalog." });
            }
        }
    }
}
