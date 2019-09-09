using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using StorageAPI.Models;
using StorageAPI.Services;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOneOrigin")]
    [ApiController]
    public class CatalogTypeController : ControllerBase
    {
        private CatalogTypeService CatalogTypeService { get; set; }

        public CatalogTypeController(IServiceProvider service)
        {
            CatalogTypeService = service.GetRequiredService<CatalogTypeService>();

        }

        // GET: api/CatalogType
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetCatalogTypes()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value;
            return Ok(await CatalogTypeService.GetAllCatalogTypes(userId));
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetCatalogTypeById(Guid id)
        {
            return Ok(await CatalogTypeService.GetCatalogTypeById(id));
        }

        // POST: api/CatalogType
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> SaveCatalogType([FromBody] CatalogType catalogType)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value;
            return Ok(await CatalogTypeService.SaveCatalogType(catalogType, userId));
        }

        // PUT: api/CatalogType/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> DeleteCatalogType(Guid id)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value;

            await CatalogTypeService.DeleteCatalogType(id, userId);
            return Ok();
        }
    }
}
