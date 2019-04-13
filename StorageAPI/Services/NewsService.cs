using StorageAPI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StorageAPI.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace StorageAPI.Services
{
    public class NewsService
    {
        protected StorageContext DB { get; private set; }

        public NewsService(IServiceProvider services)
        {
            DB = services.GetService<StorageContext>();
        }

        /// <summary>
        /// Method gets warehouse news by warehouse id
        /// </summary>
        /// <param name="id">Id of an warehouse that news we want to get</param>
        /// <returns>Warehouse from DB</returns>
        public async Task<List<News>> GetWarehouseNewsById(Guid id)
        {
            // Getting warehouse news from DB
            var warehouseNews = await DB.NewsDB.Where(x => x.WarehouseId == id).ToListAsync();
            // Checking if it's not null
            if (warehouseNews == null)
            {
                // If it's null, then we will throw new exception
                throw new Exception("Not found");
            }
            // If object was found, then we return it
            else
            {
                return warehouseNews;
            }
        }

        /// <summary>
        /// Method add or modifies a news
        /// </summary>
        /// <param name="news">news object that we want to add or modifie</param>
        /// <returns>New news with id included or modified object</returns>
        public async Task<News> SaveNews(News news)
        {

            // If warehouse does not have id, that means that it's a new entity, and we need an add functionality
            if (news.Id == null || news.Id.Equals(Guid.Empty))
            {
                // Adding new warehouse to DB
                DB.NewsDB.Add(news);
                // Saving changes in DB
                await DB.SaveChangesAsync();

            }
            // If warehouse has an id, that means that it's  not a new entity, and we need an edit functionality
            else
            {

                // Getting object from DB that has similar id like in our param variable
                var newsFromDb = await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == news.Id);
                // Using mapper to edit all fields
                Mapper.Map(news, newsFromDb);
                // Updating DB
                DB.WarehouseDB.Update(newsFromDb);
                // Saving changes in DB
                await DB.SaveChangesAsync();
            }
            // Returning object
            return news;
        }
    }
}
