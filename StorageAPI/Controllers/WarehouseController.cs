﻿using System;
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
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOneOrigin")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
      private WarehouseService WarehouseService { get; set; }
       private SimpleLogTableServcie SimpleLogTableService { get; set; }

        protected StorageContext DB { get; private set; }


        public WarehouseController(IServiceProvider service)
        {
            WarehouseService = service.GetRequiredService<WarehouseService>();
            DB = service.GetRequiredService<StorageContext>();
            SimpleLogTableService = service.GetRequiredService<SimpleLogTableServcie>();
        }

        #region Base crud
        /// <summary>
        /// Method gets all Warehouses from DB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Warehouse>>> GetAllWarehouses()
        {
            return await DB.WarehouseDB.Include(x => x.News).ToListAsync();
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
        /// Method adds or modifies a  warehouse in DB
        /// </summary>
        /// <param name="warehouse">Warehouse object that we want to add or edit</param>
        /// <returns> Warehouse object</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> CreateWarehouse([FromBody] Warehouse warehouse)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
            // Adding new warehouse by calling a method that will add it to DB
            var newWarehouse = await WarehouseService.SaveWarehouse(warehouse, username);
                // Returning new warehouse
                return Ok(newWarehouse);
        }

        /// <summary>
        /// Method deletes warehouse from DB
        /// </summary>
        /// <param name="id">Id of an warehouse that we want to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> DeleteWarehouse(Guid id)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
            if (await DB.CatalogDB.AnyAsync(x => x.WarehouseId == id))
            {
                return BadRequest(new { message = "Noliktava vēl ir produkti." });

            }
            // Calling method that will delete warehouse from DB
            await WarehouseService.DeleteWarehouse(id, username);
            // Returning filtered warehouse list
            return Ok();
        }
        #endregion Base crud

        /// <summary>
        /// Method filters warehouses by filter option
        /// </summary>
        /// <param name="option">Filter option, by which filtration will be done</param>
        /// <returns>Filtered warehouse list</returns>
        [HttpPost("filteredWarehouse")]
        public async Task<ActionResult> FilterWarehouses([FromBody] FilterSorting filterSorting)
        {

            // Filtering warehouse query by calling method that will filter it
            var newWarehouse = await WarehouseService.FilterWarehouses(filterSorting);
            // Returning filtered warehouse list
            return Ok(newWarehouse);
        }



    }
}
