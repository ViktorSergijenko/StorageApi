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
    public class CatalogController : ControllerBase
    {
        private CatalogService CatalogService { get; set; }
        protected StorageContext DB { get; private set; }
        public CatalogController(IServiceProvider service)
        {
            CatalogService = service.GetRequiredService<CatalogService>();
            DB = service.GetRequiredService<StorageContext>();
        }


        /// <summary>
        /// Method gets all catalogs from DB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Catalog>>> GetAllCatalogs()
        {
            return await DB.CatalogDB.ToListAsync();
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
        /// Method adds or modifies a  Catalog in DB
        /// </summary>
        /// <param name="Catalog">Catalog object that we want to add or edit</param>
        /// <returns> Catalog object</returns>
        [HttpPost]
        public async Task<ActionResult> CreateCatalog([FromBody] Catalog Catalog)
        {
            // Adding new Catalog by calling a method that will add it to DB
            var newCatalog = await CatalogService.SaveCatalog(Catalog);
            // Returning new Catalog
            return Ok(newCatalog);
        }

        /// <summary>
        /// Method deletes Catalog from DB
        /// </summary>
        /// <param name="id">Id of an Catalog that we want to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> FilterCatalogs(Guid id)
        {
            // Calling method that will delete Catalog from DB
            await CatalogService.DeleteCatalog(id);
            // Returning filtered Catalog list
            return Ok();
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
}
