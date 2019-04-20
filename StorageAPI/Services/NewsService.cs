﻿using StorageAPI.Context;
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
        /// Method deletes news from DB
        /// </summary>
        /// <param name="id">Id of an news to delete</param>
        /// <returns></returns>
        public async Task DeleteNews(Guid id)
        {
            // Getting warehouse from DB with the same id like in param
            var news = await DB.NewsDB.FirstOrDefaultAsync(x => x.Id == id);
            // Checkinf if warehouse variable for null
            if (news == null)
            {
                // If it's null then we throw exception
                throw new Exception("Not found");
            }
            // Removing warehouse from DB
            DB.NewsDB.Remove(news);
            // Saving changes
            await DB.SaveChangesAsync();
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
                var house = await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == news.WarehouseId);
                house.HasProblems = true;
                // Adding new warehouse to DB
                DB.NewsDB.Add(news);
                DB.WarehouseDB.Update(house);


                // Saving changes in DB
                await DB.SaveChangesAsync();

            }
            // If warehouse has an id, that means that it's  not a new entity, and we need an edit functionality
            else
            {

                // Getting object from DB that has similar id like in our param variable
                var newsFromDb = await DB.NewsDB.FirstOrDefaultAsync(x => x.Id == news.Id);
                // Using mapper to edit all fields
                Mapper.Map(news, newsFromDb);
                // Updating DB
                DB.NewsDB.Update(newsFromDb);
                // Saving changes in DB
                await DB.SaveChangesAsync();
            }
            // Returning object
            return news;
        }

        public async Task<News> toggleNewsFixedProblemFlag(Guid id)
        {
            // Getting warehouse news from DB
            var warehouseNews = await DB.NewsDB.Include(o => o.Warehouse).FirstOrDefaultAsync(x => x.Id == id);
            // Checking if it's not null
            if (warehouseNews == null)
            {
                // If it's null, then we will throw new exception
                throw new Exception("Not found");
            }
            // If object was found, then we return it
            else
            {
                // Cahnging flag value to true
                warehouseNews.FixedProblem = true;
                // Updating DB
                DB.NewsDB.Update(warehouseNews);
                // Saving changes in DB, because next step will also include DB operations
                await DB.SaveChangesAsync();
                // Checking does house in that we just toggled flag, still hase some news/problems that was not resolved
                var hasProblems = await DB.WarehouseDB.AnyAsync(x => x.News.Any(o => o.FixedProblem != true));
                if (hasProblems)
                {
                    // If has, then we get this house and change it flag hasProblems to true
                   var house =  await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == warehouseNews.WarehouseId);
                    house.HasProblems = true;
                    DB.WarehouseDB.Update(house);

                }
                else
                {
                    // If that was the last news that has not been resolved, then we change has problems flag to false
                    var house = await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == warehouseNews.WarehouseId);
                    house.HasProblems = false;
                    DB.WarehouseDB.Update(house);
                }
                // Saving changes in DB
                await DB.SaveChangesAsync();
                return warehouseNews;
            }
        }
    }
}
