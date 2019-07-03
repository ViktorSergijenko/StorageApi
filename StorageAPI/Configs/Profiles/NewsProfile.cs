using AutoMapper;
using StorageAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Configs.Profiles
{
    public class NewsProfile : Profile
    {
        public NewsProfile()
        {
            CreateMap<News, News>()
                .ForMember(x => x.Author, o => o.Ignore())
                .ForMember(x => x.NewsComments, o => o.Ignore())
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.FixedDate, o => o.Ignore())
                .ForMember(x => x.IsDeleted, o => o.Ignore())

            ;
            
        }
    }
}
