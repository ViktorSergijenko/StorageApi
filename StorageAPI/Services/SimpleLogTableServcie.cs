using StorageAPI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StorageAPI.Models;

namespace StorageAPI.Services
{
    public class SimpleLogTableServcie
    {
        protected StorageContext DB { get; private set; }
        public SimpleLogTableServcie(IServiceProvider service)
        {
            DB = service.GetService<StorageContext>();
        }

        public async Task AddLog(string action, string username)
        {
            var newLog = new SimpleLogTable() {
                UserName = username,
                Action = action,
                Date = DateTime.Now
                
            };
            await DB.SimpleLogTableDB.AddAsync(newLog);
            await DB.SaveChangesAsync();
        }

        public async Task AddAdminLog(string action, string username)
        {
            var newLog = new AdminLogTable()
            {
                UserName = username,
                Action = action,
                Date = DateTime.Now

            };
            await DB.AdminLogTable.AddAsync(newLog);
            await DB.SaveChangesAsync();
        }
    }
}
