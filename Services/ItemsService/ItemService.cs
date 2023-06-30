using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Goods.Data;
using Goods.Dtos.Items;
using Goods.Services.UserService;
using Microsoft.EntityFrameworkCore;

namespace Goods.UserService
{
    public class ItemService : IItemsService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public ItemService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<ServiceResponse<List<GetItemsDto>>> GetAllItems(CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<List<GetItemsDto>>();
            var dbItems = await _context.Items.ToListAsync();
            if (dbItems.Count < 1)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "There are no items";
                return serviceResponse;
            }
            serviceResponse.Data = dbItems.Select(c => _mapper.Map<GetItemsDto>(c)).ToList();
            return serviceResponse;
        }
        public async Task<ServiceResponse<List<GetItemsDto>>> GetItemByFilter(string columnName, string value, CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<List<GetItemsDto>>();
            var dbItems = new List<Goods.Models.Items>();

            if (columnName != "ItemName" && columnName != "ItemCode" && columnName != "Active")
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Not a valid column name";
                return serviceResponse;
            }

            if (columnName == "ItemName")
            {
                dbItems = dbItems = await _context.Items.Where(c => c.ItemName == value).ToListAsync();
            }

            if (columnName == "ItemCode")
            {
                dbItems = await _context.Items.Where(c => c.ItemCode == value).ToListAsync();
            }
            else
            {
                dbItems = await _context.Items.Where(c => c.Active == (value != "0")).ToListAsync();
            }

            serviceResponse.Data = dbItems.Select(c => _mapper.Map<GetItemsDto>(c)).ToList();
            return serviceResponse;
        }
    }
}