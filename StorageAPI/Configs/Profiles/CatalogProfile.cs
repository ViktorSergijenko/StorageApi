using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using StorageAPI.Models;

namespace StorageAPI.Configs.Profiles
{
    public class CatalogProfile : Profile
    {
        CatalogProfile() {
            CreateMap<Catalog, Catalog>();

        }
    }
}
