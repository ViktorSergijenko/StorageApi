using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StorageAPI.Models;
using AutoMapper;

namespace StorageAPI.Configs.Profiles
{
    public class WarehouseProfile : Profile
    {
        public WarehouseProfile()
        {
            CreateMap<Warehouse, Warehouse>()
                .ForMember(x => x.QrCodeBase64, o => o.Ignore());
        }
    }
}
