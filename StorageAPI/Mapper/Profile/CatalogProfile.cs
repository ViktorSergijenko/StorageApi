﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using StorageAPI.Models;

namespace StorageAPI.Mapper.Profile
{
    public class CatalogProfile : AutoMapper.Profile
    {
        CatalogProfile() {
            CreateMap<Catalog, Catalog>();
        }
    }
}
