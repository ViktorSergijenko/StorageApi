using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using StorageAPI.Configs.Profiles;

namespace StorageAPI.Configs
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings(IServiceProvider services)
        {
            Mapper.Initialize(c =>
            {
                // This is needed for automapper to be able create models
                // with injectable services
                c.AddProfile<WarehouseProfile>();

            });
        }
    }
}
