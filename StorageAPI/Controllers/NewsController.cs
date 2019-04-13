using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StorageAPI.Context;
using StorageAPI.Models;
using StorageAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOneOrigin")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private NewsService NewsService { get; set; }
        protected StorageContext DB { get; private set; }

        public NewsController(IServiceProvider service)
        {
            NewsService = service.GetRequiredService<NewsService>();
            DB = service.GetRequiredService<StorageContext>();
        }

        #region Base crud
        /// <summary>
        /// Method gets all news from DB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<News>>> GetAllNews()
        {
            return await DB.NewsDB.ToListAsync();
        }

        /// <summary>
        /// Method gets warehouse news by id
        /// </summary>
        /// <param name="id">Id of a warehouse that we want to get</param>
        /// <returns>Ok status with an warehouse object</returns>
        [HttpGet("{warehouseId}")]
        public async Task<ActionResult> GetWarehouseNewsById(Guid warehouseId)
        {
            // Getting warehouse news by id
            var warehouse = await NewsService.GetWarehouseNewsById(warehouseId);
            // Returning warehouse news
            return Ok(warehouse);
        }

        /// <summary>
        /// Method togles news flag by id
        /// </summary>
        /// <param name="id">Id of a news that we want to toggle</param>
        /// <returns>Ok status with an news object</returns>
        [HttpGet("fix-news/{warehouseId}")]
        public async Task<ActionResult> ToggleFixedNewsFlag(Guid warehouseId)
        {
            // Getting warehouse news by id
            var warehouse = await NewsService.toggleNewsFixedProblemFlag(warehouseId);
            // Returning warehouse news
            return Ok(warehouse);
        }

        /// <summary>
        /// Method adds or modifies a object in DB
        /// </summary>
        /// <param name="news">News object that we want to add or edit</param>
        /// <returns> Edited or created  object</returns>
        [HttpPost]
        public async Task<ActionResult> SaveOrCreate([FromBody] News news)
        {
            // Adding new object by calling a method that will add it to DB
            var editedOrCreatedNews = await NewsService.SaveNews(news);
            // Returning new object
            return Ok(editedOrCreatedNews);
        }
        #endregion Base crud
    }
}
