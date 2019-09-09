using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StorageAPI.Models;
using AutoMapper;
using StorageAPI.ModelsVM;

namespace StorageAPI.Configs.Profiles
{
    public class WarehouseProfile : Profile
    {
        public WarehouseProfile()
        {
            CreateMap<Warehouse, Warehouse>()
                .ForMember(x => x.QrCodeBase64, o => o.Ignore());
            CreateMap<WarehouseVM, Warehouse>();
            CreateMap<UserWarehouse, WarehouseVM>()
                .ForMember(x => x.Id, o => o.MapFrom(x => x.Warehouse.Id))
                .ForMember(x => x.HasProblems, o => o.MapFrom(x => x.Warehouse.HasProblems))
                .ForMember(x => x.Name, o => o.MapFrom(x => x.Warehouse.Name))
                .ForMember(x => x.Address, o => o.MapFrom(x => x.Warehouse.Address))
                .ForMember(x => x.HasMinCatalogs, o => o.MapFrom(x => x.Warehouse.HasMinCatalogs))

                //.ForMember(x => x.WarehousePositionInTable, o => o.MapFrom(x => x.WarehousePositionInTable))
                ;
        }
    }
}
