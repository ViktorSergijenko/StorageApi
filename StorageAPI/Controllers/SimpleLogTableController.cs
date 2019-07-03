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
        [HttpPost("filter")]
        public async Task<ActionResult> GetFilteredSimpleLogs([FromBody] DateFiltration dateFiltration)
        {
            var currentTime = DateTime.Now;
            if (dateFiltration.LastHour)
            {
                return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddHours(-1) && x.Date <= currentTime).ToListAsync()); 
            }
            if (dateFiltration.LastThirtyMinutes)
            {
                return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddMinutes(-30) && x.Date <= currentTime).ToListAsync()); 
            }
            if (dateFiltration.LastSixHours)
            {
                return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddHours(-6) && x.Date <= currentTime).ToListAsync()); 
            }
            if (dateFiltration.LastWeek)
            {
                return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddDays(-7) && x.Date <= currentTime).ToListAsync()); 
            }
            if (dateFiltration.LastMonth)
            {
                return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= currentTime.AddMonths(-1) && x.Date <= currentTime).ToListAsync()); 
            }
            if (dateFiltration.TimeTill != null && dateFiltration.TimeFrom != null)
            {
                return Ok(await DB.SimpleLogTableDB.Where(x => x.Date >= dateFiltration.TimeFrom && x.Date <= dateFiltration.TimeTill).ToListAsync());
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
