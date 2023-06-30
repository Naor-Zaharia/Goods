using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goods.Dtos.BusinessPartners;

namespace Goods.Services.BusinessPartnersService
{
    public interface IBusinessPartnersService
    {
        Task<ServiceResponse<List<GetBusinessPartnersDto>>> GetAllBusinessPartners(CancellationToken token);
        Task<ServiceResponse<List<GetBusinessPartnersDto>>> GetBusinessPartnersByFilter(string columnName, string value, CancellationToken token);
    }
}