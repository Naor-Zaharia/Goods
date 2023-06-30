using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goods.Dtos.Items;

namespace Goods.Services.UserService
{
    public interface IItemsService
    {
        Task<ServiceResponse<List<GetItemsDto>>> GetAllItems(CancellationToken token);
        Task<ServiceResponse<List<GetItemsDto>>> GetItemByFilter(string columnName, string value, CancellationToken token);
    }
}