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
        public async Task<ActionResult<List<SimpleLogTableController>>> GetLogs()
        {
            var list = await DB.SimpleLogTableDB.ToListAsync();
            list = list.OrderByDescending(x => x.Date).ToList();
            return Ok(list);
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
