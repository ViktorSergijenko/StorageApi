using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StorageAPI.Services;
using StorageAPI.Context;
using StorageAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOneOrigin")]
    [ApiController]
    public class CatalogNameController : ControllerBase
    {
        private CatalogNameService CatalogNameService { get; set; }
        protected StorageContext DB { get; private set; }
        private SimpleLogTableServcie SimpleLogTableService { get; set; }


        public CatalogNameController(IServiceProvider service)
        {
            CatalogNameService = service.GetRequiredService<CatalogNameService>();
            DB = service.GetRequiredService<StorageContext>();
            SimpleLogTableService = service.GetRequiredService<SimpleLogTableServcie>();

        }


        // GET: api/CatalogName/5
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var cataqlogNameList = await CatalogNameService.GetCatalogNameList();
            if (cataqlogNameList.Count == 0)
            {
                return BadRequest(new { message = "There is no catalog names." });
            }
            return Ok(cataqlogNameList);
        }

        // POST: api/CatalogName
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> SaveCatalogName([FromBody] CatalogName catalogName)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
            var name = await CatalogNameService.SaveCataogName(catalogName, username);
            return Ok(name);
        }
        [HttpPost("filteredCatalogName")]
        public async Task<ActionResult> FilterWarehouses([FromBody] FilterSorting filterSorting)
        {

            // Filtering warehouse query by calling method that will filter it
            var filteredCatalogList = await CatalogNameService.FilterCatalogName(filterSorting);
            // Returning filtered warehouse list
            return Ok(filteredCatalogList);
        }


        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;

            var objectToDelete = DB.CatalogNameDB.FirstOrDefault(x => x.Id == id);
            var checkingCatalogsWithThisName = await DB.CatalogDB.AnyAsync(x => x.CatalogNameId == objectToDelete.Id);
            if (checkingCatalogsWithThisName)
            {
                return BadRequest(new { message = "Cant delete this catalog name, since there are catalogs with this name." });
            }
            else
            {
                DB.CatalogNameDB.Remove(objectToDelete);
                await SimpleLogTableService.AddAdminLog($"Nodzesa kataloga nosaukumu: {objectToDelete.Name}", username);

            }
            return Ok();
        }
    }
}
