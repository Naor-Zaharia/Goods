using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Goods.Data;
using Goods.Dtos.BusinessPartners;
using Goods.Services.BusinessPartnersService;
using Microsoft.EntityFrameworkCore;

namespace Goods.Services.BusinessPartnersService
{
    public class BusinessPartnersService : IBusinessPartnersService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public BusinessPartnersService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetBusinessPartnersDto>>> GetAllBusinessPartners(CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<List<GetBusinessPartnersDto>>();
            var dbBusinessPartners = await _context.BusinessPartners.ToListAsync();
            if (dbBusinessPartners.Count < 1)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "There are no business partners ";
                return serviceResponse;
            }
            serviceResponse.Data = dbBusinessPartners.Select(c => _mapper.Map<GetBusinessPartnersDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetBusinessPartnersDto>>> GetBusinessPartnersByFilter(string columnName, string value, CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<List<GetBusinessPartnersDto>>();
            var dbBusinessPartners = new List<Goods.Models.BusinessPartners>();

            if (columnName != "BPName" && columnName != "BPType" && columnName != "Active")
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Not a valid column name";
                return serviceResponse;
            }

            if (columnName == "BPName")
            {
                dbBusinessPartners = await _context.BusinessPartners.Where<BusinessPartners>(c => c.BPName == value).ToListAsync();
            }

            if (columnName == "BPType")
            {
                dbBusinessPartners = await _context.BusinessPartners.Where(c => c.BPType == char.Parse(value)).ToListAsync();
            }
            else
            {
                dbBusinessPartners = await _context.BusinessPartners.Where<BusinessPartners>(c => c.Active == (value != "0")).ToListAsync();
            }

            serviceResponse.Data = dbBusinessPartners.Select(c => _mapper.Map<GetBusinessPartnersDto>(c)).ToList();
            return serviceResponse;
        }
    }
}