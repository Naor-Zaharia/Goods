using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Goods.Dtos.Items;
using Goods.Dtos.BusinessPartners;
using Goods.Dtos.Document;
using System.Runtime.CompilerServices;




namespace Goods
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BusinessPartners, GetBusinessPartnersDto>();
            CreateMap<Items, GetItemsDto>();
        }
    }
}