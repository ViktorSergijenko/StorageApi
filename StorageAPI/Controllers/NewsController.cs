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
using Microsoft.AspNetCore.Authorization;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOneOrigin")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private NewsService NewsService { get; set; }
        protected StorageContext DB { get; private set; }
        private SimpleLogTableServcie SimpleLogTableService { get; set; }

        public NewsController(IServiceProvider service)
        {
            NewsService = service.GetRequiredService<NewsService>();
            DB = service.GetRequiredService<StorageContext>();
            SimpleLogTableService = service.GetRequiredService<SimpleLogTableServcie>();

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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> ToggleFixedNewsFlag(Guid warehouseId)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;

            // Getting warehouse news by id
            var news = await NewsService.toggleNewsFixedProblemFlag(warehouseId);
            await SimpleLogTableService.AddLog($"Resolved problems related with: {news.Title}", username);

            // Returning warehouse news
            return Ok(news);
        }

        /// <summary>
        /// Method deletes news from DB
        /// </summary>
        /// <param name="id">Id of an warehouse that we want to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Delete(Guid id)
        {
            // Calling method that will delete news from DB
            var news = await NewsService.DeleteNews(id);
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
            await SimpleLogTableService.AddAdminLog($"Deleted problem related with: {news.Title}", username);
            // Returning ok status
            return Ok();
        }
        /// <summary>
        /// Method adds or modifies a object in DB
        /// </summary>
        /// <param name="news">News object that we want to add or edit</param>
        /// <returns> Edited or created  object</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> SaveOrCreate([FromBody] News news)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
            // Adding new object by calling a method that will add it to DB
            var editedOrCreatedNews = await NewsService.SaveNews(news, username);
            // Returning new object
            return Ok(editedOrCreatedNews);
        }
        #endregion Base crud
    }
}
