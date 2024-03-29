﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using StorageAPI.Models;

namespace StorageAPI.Configs.Profiles
{
    public class CatalogProfile : Profile
    {
       public CatalogProfile() {
            CreateMap<Catalog, Catalog>()
              .ForMember(x => x.Products, o => o.Ignore());

            CreateMap<Catalog, CatalogName>()
             .ForMember(x => x.Name, o => o.MapFrom(x => x.Name.Name));

        }
    }
}
