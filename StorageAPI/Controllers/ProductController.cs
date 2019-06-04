using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using StorageAPI.Services;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOneOrigin")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private ProductService ProductService { get; set; }
        public ProductController(IServiceProvider service)
        {
            ProductService = service.GetRequiredService<ProductService>();
        }
        /// <summary>
        /// Method gets products from specifick catalog
        /// </summary>
        /// <param name="id">Id of a Catalog from that we want to get products</param>
        /// <returns>Ok status with an product list</returns>
        [HttpGet("catalog/{id}")]
        public async Task<ActionResult> GetProductFromSpecifickCatalog(Guid id)
        {
            // Getting products by catalog id
            var products = await ProductService.GetProductByCatalogId(id);
            // Returning Products 
            return Ok(products);
        }
    }
}
