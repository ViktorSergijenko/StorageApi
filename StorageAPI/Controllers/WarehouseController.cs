using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StorageAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using StorageAPI.Context;
using StorageAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAnyOrigin")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
      private WarehouseService WarehouseService { get; set; }
        protected StorageContext DB { get; private set; }

        public WarehouseController(IServiceProvider service)
        {
            WarehouseService = service.GetService<WarehouseService>();
            DB = service.GetService<StorageContext>();
        }

        [HttpGet]
        public async Task<ActionResult<List<Warehouse>>> GetAllWarehouses()
        {
            return await DB.WarehouseDB.ToListAsync();
        }

        /// <summary>
        /// Method gets warehouse by id
        /// </summary>
        /// <param name="id">Id of a warehouse that we want to get</param>
        /// <returns>Ok status with an warehouse object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetWarehouseById(Guid id)
        {
            // Getting warehouse by id
            var warehouse = await WarehouseService.GetWarehouseById(id);
            // Returning warehouse 
            return Ok(warehouse);
        }

        /// <summary>
        /// Method adds a new warehouse in to DB
        /// </summary>
        /// <param name="warehouse">Warehouse object that we want to add</param>
        /// <returns>New warehouse</returns>
        [HttpPost]
        public async Task<ActionResult> CreateWarehouse([FromBody] Warehouse warehouse)
        {
                // Adding new warehouse by calling a method that will add it to DB
                var newWarehouse = await WarehouseService.AddWarehouse(warehouse);
                // Returning new warehouse
                return Ok(newWarehouse);
        }

        /// <summary>
        /// Method filters warehouses by filter option
        /// </summary>
        /// <param name="option">Filter option, by which filtration will be done</param>
        /// <returns>Filtered warehouse list</returns>
        [HttpPost("filteredWarehouse")]
        public async Task<ActionResult> FilterWarehouses([FromBody] string option)
        {
            // Filtering warehouse query by calling method that will filter it
            var newWarehouse = await WarehouseService.FilterWarehouses(option);
            // Returning filtered warehouse list
            return Ok(newWarehouse);
        }


    }
}
