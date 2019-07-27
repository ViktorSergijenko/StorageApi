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
        [HttpPost]
        public async Task<ActionResult<List<SimpleLogTable>>> GetLogs([FromBody] DateFiltration dateFiltration)
        {
            var currentTime = DateTime.Now;
            if (dateFiltration.TimeFrom == null &&
                dateFiltration.TimeTill == null &&
                dateFiltration.LastHour == false &&
                dateFiltration.LastDay == false &&
                dateFiltration.LastWeek == false &&
                dateFiltration.LastMonth == false)
            {
                return Ok(await DB.SimpleLogTableDB
                    .Where(x => x.Date >= currentTime.AddMonths(-1) && x.Date <= currentTime)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync());
            }
            if (dateFiltration.LastHour)
            {
                return Ok(await DB.SimpleLogTableDB
                    .Where(x => x.Date >= currentTime.AddHours(-1) && x.Date <= currentTime)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync());
            }
            if (dateFiltration.LastDay)
            {
                return Ok(await DB.SimpleLogTableDB
                    .Where(x => x.Date >= currentTime.AddDays(-1) && x.Date <= currentTime)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync());
            }
            if (dateFiltration.LastWeek)
            {
                var a = await DB.SimpleLogTableDB
                    .Where(x => x.Date >= currentTime.AddDays(-7) && x.Date <= currentTime)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();
                return Ok(await DB.SimpleLogTableDB
                    .Where(x => x.Date >= currentTime.AddDays(-7) && x.Date <= currentTime)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync());
            }
            if (dateFiltration.LastMonth)
            {
                return Ok(await DB.SimpleLogTableDB
                    .Where(x => x.Date >= currentTime.AddMonths(-1) && x.Date <= currentTime)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync());
            }
            if (dateFiltration.TimeTill != null || dateFiltration.TimeFrom != null)
            {
                if (dateFiltration.TimeFrom != null && dateFiltration.TimeTill == null)
                {
                    var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= from && x.Date <= from.AddDays(+1))
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                else
                {
                    var till = dateFiltration.TimeTill.Value.AddDays(+1).AddHours(+3);
                    var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                    return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= from && x.Date <= till)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
            }
            return Ok();
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
            var list = await DB.SimpleLogTableDB.Where(x => x.WarehouseId == id && x.UserName == username && x.Date >= currentTime.AddHours(-12) && x.Date <= currentTime && x.Action == "Pievienots").ToListAsync();
            list = list.OrderByDescending(x => x.Date).ToList();
            return Ok(list);
        }

        [HttpPost("filter")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetFilteredSimpleLogs([FromBody] DateFiltration dateFiltration)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type == "Role").Value;
            var userIdThatWantToRecieveLogs = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value;

            var userThatWantToRecieveLogs = await DB.Users.FirstOrDefaultAsync(x => x.Id == userIdThatWantToRecieveLogs);
            List<string> userIds = new List<string>();
            #region Level one
            if (role == "Level one")
            {

                var currentTime = DateTime.Now;
                if (dateFiltration.TimeFrom == null &&
                    dateFiltration.TimeTill == null &&
                    dateFiltration.LastHour == false &&
                    dateFiltration.LastDay == false &&
                    dateFiltration.LastWeek == false &&
                    dateFiltration.LastMonth == false)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddHours(-12) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastHour)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddHours(-1) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastDay)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddDays(-1) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastWeek)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddDays(-7) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastMonth)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddMonths(-1) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.TimeTill != null || dateFiltration.TimeFrom != null)
                {
                    if (dateFiltration.TimeFrom != null && dateFiltration.TimeTill == null)
                    {
                        var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                        return Ok(await DB.SimpleLogTableDB
                            .Where(x => x.Date >= from && x.Date <= from.AddDays(+1) && x.WarehouseId == dateFiltration.WarehouseId)
                            .OrderByDescending(x => x.Date)
                            .ToListAsync());
                    }
                    else
                    {
                        var till = dateFiltration.TimeTill.Value.AddDays(+1).AddHours(+3);
                        var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                        return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= from && x.Date <= till && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
                    }
                }
            }
            #endregion Level one
            #region Level two
            if (role == "Level two")
            {
                // Getting all users that serves to this employee
                userIds = await DB.Users
                    .Where(x => x.ReportsTo == userThatWantToRecieveLogs.Id || x.Id == userThatWantToRecieveLogs.Id)
                    .Select(x => x.Id)
                    .Distinct().
                    ToListAsync()
                    ;
                // Getting all level four users that serves to level three
                var employeesOfEmployees = await DB.Users.Where(x => userIds.Any(y => y == x.ReportsTo)).Select(x => x.Id).ToListAsync();
                employeesOfEmployees.ForEach(x => userIds.Add(x));
                var userList = await DB.Users.Where(x => userIds.Any(y => y == x.Id)).Select(x => x.FullName).ToListAsync();
                var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
                var currentTime = DateTime.Now;
                if (dateFiltration.TimeFrom == null &&
                    dateFiltration.TimeTill == null &&
                    dateFiltration.LastHour == false &&
                    dateFiltration.LastWeek == false &&
                    dateFiltration.LastMonth == false)
                {

                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddHours(-12) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId && userList.Any(y => y == x.UserName))
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastHour)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddHours(-1) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId && userList.Any(y => y == x.UserName))
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastWeek)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddDays(-7) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId && userList.Any(y => y == x.UserName))
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastMonth)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddMonths(-1) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId && userList.Any(y => y == x.UserName))
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.TimeTill != null || dateFiltration.TimeFrom != null)
                {
                    if (dateFiltration.TimeFrom != null && dateFiltration.TimeTill == null)
                    {
                        var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                        return Ok(await DB.SimpleLogTableDB
                            .Where(x => x.Date >= from && x.Date <= from.AddDays(+1) && x.WarehouseId == dateFiltration.WarehouseId && userList.Any(y => y == x.UserName))
                            .OrderByDescending(x => x.Date)
                            .ToListAsync());
                    }
                    else
                    {
                        var till = dateFiltration.TimeTill.Value.AddDays(+1).AddHours(+3);
                        var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                        return Ok(await DB.SimpleLogTableDB
                            .Where(x => x.Date >= from && x.Date <= till && x.WarehouseId == dateFiltration.WarehouseId && userList.Any(y => y == x.UserName))
                            .OrderByDescending(x => x.Date)
                            .ToListAsync());
                    }
                }
            }
            #endregion Level two
            #region Level three
            if (role == "Level three")
            {
                // Getting all users that serves to this employee
                userIds = await DB.Users
                    .Where(x => x.ReportsTo == userThatWantToRecieveLogs.Id || x.Id == userThatWantToRecieveLogs.Id)
                    .Select(x => x.Id)
                    .Distinct().
                    ToListAsync()
                    ;
                var userList = await DB.Users.Where(x => userIds.Any(y => y == x.Id)).Select(x => x.FullName).ToListAsync();
                var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
                var currentTime = DateTime.Now;
                if (dateFiltration.TimeFrom == null &&
                    dateFiltration.TimeTill == null &&
                    dateFiltration.LastHour == false &&
                    dateFiltration.LastWeek == false &&
                    dateFiltration.LastMonth == false)
                {

                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddHours(-12) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId && userList.Any(y => y == x.UserName))
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastHour)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddHours(-1) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId && userList.Any(y => y == x.UserName))
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastWeek)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddDays(-7) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId && userList.Any(y => y == x.UserName))
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastMonth)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddMonths(-1) && x.Date <= currentTime && x.WarehouseId == dateFiltration.WarehouseId && userList.Any(y => y == x.UserName))
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.TimeTill != null || dateFiltration.TimeFrom != null)
                {
                    if (dateFiltration.TimeFrom != null && dateFiltration.TimeTill == null)
                    {
                        var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                        return Ok(await DB.SimpleLogTableDB
                            .Where(x => x.Date >= from && x.Date <= from.AddDays(+1) && x.WarehouseId == dateFiltration.WarehouseId && userList.Any(y => y == x.UserName))
                            .OrderByDescending(x => x.Date)
                            .ToListAsync());
                    }
                    else
                    {
                        var till = dateFiltration.TimeTill.Value.AddDays(+1).AddHours(+3);
                        var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                        return Ok(await DB.SimpleLogTableDB
                            .Where(x => x.Date >= from && x.Date <= till && x.WarehouseId == dateFiltration.WarehouseId && userList.Any(y => y == x.UserName))
                            .OrderByDescending(x => x.Date)
                            .ToListAsync());
                    }
                }
            }
            #endregion Level three
            #region Level four
            if (role == "Level four")
            {
                var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
                var currentTime = DateTime.Now;
                if (dateFiltration.TimeFrom == null &&
                    dateFiltration.TimeTill == null &&
                    dateFiltration.LastHour == false &&
                    dateFiltration.LastWeek == false &&
                    dateFiltration.LastMonth == false)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddDays(-1) && x.Date <= currentTime  && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastHour)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddHours(-1) && x.Date <= currentTime && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastWeek)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddDays(-7) && x.Date <= currentTime && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.LastMonth)
                {
                    return Ok(await DB.SimpleLogTableDB
                        .Where(x => x.Date >= currentTime.AddMonths(-1) && x.Date <= currentTime && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                if (dateFiltration.TimeTill != null || dateFiltration.TimeFrom != null)
                {
                    if (dateFiltration.TimeFrom != null && dateFiltration.TimeTill == null)
                    {
                        var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                        return Ok(await DB.SimpleLogTableDB
                            .Where(x => x.Date >= from && x.Date <= from.AddDays(+1) && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId)
                            .OrderByDescending(x => x.Date)
                            .ToListAsync());
                    }
                    else
                    {
                        var till = dateFiltration.TimeTill.Value.AddDays(+1).AddHours(+3);
                        var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                        return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= from && x.Date <= till && x.UserName == username && x.WarehouseId == dateFiltration.WarehouseId).OrderByDescending(x => x.Date).ToListAsync());
                    }
                }
            }
            #endregion Level four
            return Ok();
        }
        [HttpPost("admin")]
        public async Task<ActionResult<List<SimpleLogTableController>>> GetAdminLogs([FromBody] DateFiltration dateFiltration)
        {
            var currentTime = DateTime.Now;
            if (dateFiltration.TimeFrom == null &&
                dateFiltration.TimeTill == null &&
                dateFiltration.LastHour == false &&
                dateFiltration.LastDay == false &&
                dateFiltration.LastWeek == false &&
                dateFiltration.LastMonth == false)
            {
                return Ok(await DB.AdminLogTable
                    .Where(x => x.Date >= currentTime.AddMonths(-1) && x.Date <= currentTime)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync());
            }
            if (dateFiltration.LastHour)
            {
                return Ok(await DB.AdminLogTable
                    .Where(x => x.Date >= currentTime.AddHours(-1) && x.Date <= currentTime)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync());
            }
            if (dateFiltration.LastDay)
            {
                return Ok(await DB.AdminLogTable
                    .Where(x => x.Date >= currentTime.AddDays(-1) && x.Date <= currentTime)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync());
            }
            if (dateFiltration.LastWeek)
            {
                var a = await DB.AdminLogTable
                    .Where(x => x.Date >= currentTime.AddDays(-7) && x.Date <= currentTime)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();
                return Ok(await DB.AdminLogTable
                    .Where(x => x.Date >= currentTime.AddDays(-7) && x.Date <= currentTime)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync());
            }
            if (dateFiltration.LastMonth)
            {
                return Ok(await DB.AdminLogTable
                    .Where(x => x.Date >= currentTime.AddMonths(-1) && x.Date <= currentTime)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync());
            }
            if (dateFiltration.TimeTill != null || dateFiltration.TimeFrom != null)
            {
                if (dateFiltration.TimeFrom != null && dateFiltration.TimeTill == null)
                {
                    var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                    return Ok(await DB.AdminLogTable
                        .Where(x => x.Date >= from && x.Date <= from.AddDays(+1))
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
                else
                {
                    var till = dateFiltration.TimeTill.Value.AddDays(+1).AddHours(+3);
                    var from = dateFiltration.TimeFrom.Value.AddDays(+1).AddHours(-21);
                    return Ok(await DB.AdminLogTable.Where(x => x.Date >= from && x.Date <= till)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync());
                }
            }
            return Ok();
        }
    }
}
