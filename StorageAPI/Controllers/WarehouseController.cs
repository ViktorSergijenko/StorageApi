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
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using StorageAPI.ModelsVM;
using AutoMapper.QueryableExtensions;

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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<List<WarehouseVM>>> GetAllWarehouses()
        {
            var userID = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value;
            //var warehousesIds = await DB.UserWarehouseDB.Where(x => x.UserId == userID).Select(x => x.WarehouseId).ToListAsync();

            var warehouse = await DB.UserWarehouseDB.Where(x => x.UserId == userID)
                .Include(x => x.Warehouse)
                .ThenInclude(x => x.WarehouseLogs)
                .OrderByDescending(x => x.Warehouse.HasProblems)
                .ThenByDescending(x => x.Warehouse.HasMinCatalogs)
                .ThenBy(x => x.WarehousePositionInTable)
                .ToListAsync();

            //var warehouse3 = await DB.UserWarehouseDB.Where(x => x.UserId == userID).ProjectTo<WarehouseVM>().ToListAsync();


            //var warehouseList = Mapper.Map<List<WarehouseVM>>(await DB.WarehouseDB.Where(x => warehousesIds.Contains(x.Id)).ToListAsync());
            return Mapper.Map<List<WarehouseVM>>(warehouse);
        }

        [HttpGet("position")]
        public async Task<string> resolveProblemWhtPosition()
        {
            var userList = await DB.Users.Select(x => x.Id).ToListAsync();
            foreach (var userId in userList)
            {
                int position = 1;
                var userWarehouses = await DB.UserWarehouseDB.Include(x => x.Warehouse).Where(x => x.UserId == userId).ToListAsync();
                foreach (var warehouse in userWarehouses)
                {
                    warehouse.WarehousePositionInTable = position++;
                    DB.UserWarehouseDB.Update(warehouse);
                }

            }
            await DB.SaveChangesAsync();
            return "asd";
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

        [HttpPost("specific-user-warehouse")]
        public async Task<ActionResult> GetUserWarehouseById(UserWarehouse userWarehouse)
        {
            // Getting warehouse by id
            var specificUserWarehouse = await DB.UserWarehouseDB.FirstOrDefaultAsync(x => x.WarehouseId == userWarehouse.WarehouseId && x.UserId == userWarehouse.UserId);
            // Returning warehouse 
            return Ok(specificUserWarehouse);
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
            var userId = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value;
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

        [HttpPost("add-user-to-warehouse")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> AddUserToWarehouse([FromBody] UserWarehouse userWarehouse)
        {
            return Ok(await WarehouseService.AddUserToWarehouse(userWarehouse));
        }

        [HttpPost("remove-from-warehouse")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> RemoveUserToWarehouse([FromBody] UserWarehouse userWarehouse)
        {
           var removedUser = await WarehouseService.RemoveUserToWarehouse(userWarehouse);
            return Ok(removedUser);
        }

        [HttpPost("warehouse-employees")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetUsersThatAllowToUseWarehouse([FromBody] UserWarehouse userWarehouse)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type == "Role").Value;
            var users = await WarehouseService.GetUsersThatAllowToUseWarehouse(userWarehouse, role);
            return Ok(users);
        }

        [HttpPost("toggle-amount")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> ToggleSeeAmountInWarehouse([FromBody] UserWarehouse userWarehouse)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type == "Role").Value;
            var neededUserWarehouse = await DB.UserWarehouseDB.FirstOrDefaultAsync(x => x.WarehouseId == userWarehouse.WarehouseId && x.UserId == userWarehouse.UserId);
            neededUserWarehouse.DoesUserHaveAbilityToSeeProductAmount = !neededUserWarehouse.DoesUserHaveAbilityToSeeProductAmount;
            DB.UserWarehouseDB.Update(neededUserWarehouse);
            await DB.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("user-warehouse")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetUserWarehouse([FromBody] UserWarehouse userWarehouse)
        {
            var neededUserWarehouse = await DB.UserWarehouseDB.FirstOrDefaultAsync(x => x.WarehouseId == userWarehouse.WarehouseId && x.UserId == userWarehouse.UserId);
            return Ok(neededUserWarehouse);
        }

        [HttpPost("change-position")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> ChangeWarehousePosition([FromBody] UserWarehouse userWarehouse)
        {
            await WarehouseService.ChangeWarehousePosition(userWarehouse);
            return Ok();
        }

    }
}
