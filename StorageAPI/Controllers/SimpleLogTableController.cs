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

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOneOrigin")]
    [ApiController]
    public class SimpleLogTableController : ControllerBase
    {
        public SimpleLogTableController(IServiceProvider service)
        {
            DB = service.GetRequiredService<StorageContext>();
        }
        protected StorageContext DB { get; private set; }
        /// <summary>
        /// Method gets all Warehouses from DB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<SimpleLogTable>>> GetLogs()
        {
            var list = await DB.SimpleLogTableDB.ToListAsync();
            list = list.OrderByDescending(x => x.Date).ToList();
            return Ok(list);
        }
        [HttpGet("id")]
        public async Task<ActionResult<List<SimpleLogTable>>> GetLogsByWaehouseId(Guid id)
        {
            var list = await DB.SimpleLogTableDB.Where(x => x.WarehouseId == id).ToListAsync();
            list = list.OrderByDescending(x => x.Date).ToList();
            return Ok(list);
        }
        [HttpGet("onlyMyLogs/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<List<SimpleLogTable>>> GetMyWarehouseLogs(Guid id)
        {
            var currentTime = DateTime.Now;
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
            var list = await DB.SimpleLogTableDB.Where(x => x.WarehouseId == id && x.UserName == username && x.Date >= currentTime.AddHours(-1) && x.Date <= currentTime && x.Action == "Pievienots").ToListAsync();
            list = list.OrderByDescending(x => x.Date).ToList();
            return Ok(list);
        }

        [HttpPost("filter")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetFilteredSimpleLogs([FromBody] DateFiltration dateFiltration)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type == "Role").Value;
            if (role != "Level four")
            {

            var currentTime = DateTime.Now;
            if (dateFiltration.TimeFrom == null &&
                dateFiltration.TimeTill == null &&
                dateFiltration.LastHour == false &&
                dateFiltration.LastWeek == false &&
                dateFiltration.LastMonth == false)
            {
                return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddDays(-1) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
            }
            if (dateFiltration.LastHour)
            {
                return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddHours(-1) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync()); 
            }
            if (dateFiltration.LastThirtyMinutes)
            {
                return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddMinutes(-30) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync()); 
            }
            if (dateFiltration.LastSixHours)
            {
                return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddHours(-6) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync()); 
            }
            if (dateFiltration.LastWeek)
            {
                return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddDays(-7) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync()); 
            }
            if (dateFiltration.LastMonth)
            {
                return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddMonths(-1) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync()); 
            }
            if (dateFiltration.TimeTill != null || dateFiltration.TimeFrom != null)
            {
                if (dateFiltration.TimeFrom != null && dateFiltration.TimeTill == null)
                {
                    var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                    return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= from && x.Date <= from.AddDays(+1) && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
                }
                else
                {
                    var till = dateFiltration.TimeTill.Value.AddDays(+1).AddHours(+3);
                    var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                    return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= from && x.Date <= till && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
                }
            }
            }
            else
            {
                var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
                var currentTime = DateTime.Now;
                if (dateFiltration.TimeFrom == null &&
                    dateFiltration.TimeTill == null &&
                    dateFiltration.LastHour == false &&
                    dateFiltration.LastWeek == false &&
                    dateFiltration.LastMonth == false)
                {
                    return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddDays(-1) && x.Date <= currentTime  && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
                }
                if (dateFiltration.LastHour)
                {
                    return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddHours(-1) && x.Date <= currentTime && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
                }
                if (dateFiltration.LastThirtyMinutes)
                {
                    return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddMinutes(-30) && x.Date <= currentTime && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
                }
                if (dateFiltration.LastSixHours)
                {
                    return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddHours(-6) && x.Date <= currentTime && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
                }
                if (dateFiltration.LastWeek)
                {
                    return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddDays(-7) && x.Date <= currentTime && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
                }
                if (dateFiltration.LastMonth)
                {
                    return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddMonths(-1) && x.Date <= currentTime && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
                }
                if (dateFiltration.TimeTill != null || dateFiltration.TimeFrom != null)
                {
                    if (dateFiltration.TimeFrom != null && dateFiltration.TimeTill == null)
                    {
                        var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                        return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= from && x.Date <= from.AddDays(+1) && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
                    }
                    else
                    {
                        var till = dateFiltration.TimeTill.Value.AddDays(+1).AddHours(+3);
                        var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                        return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= from && x.Date <= till && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
                    }
                }
            }
            return Ok();
        }
        [HttpGet("admin")]
        public async Task<ActionResult<List<SimpleLogTableController>>> GetAdminLogs()
        {
            var list = await DB.AdminLogTable.ToListAsync();
            list = list.OrderByDescending(x => x.Date).ToList();
            return Ok(list);
        }
    }
}
