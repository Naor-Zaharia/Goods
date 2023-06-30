using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Goods.Services.UserService;
using Goods.Dtos.Items;

namespace Goods.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsControler : ControllerBase
    {
        private readonly IItemsService _itemService;

        public ItemsControler(IItemsService itemsService)
        {
            _itemService = itemsService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetItemsDto>>>> Get(CancellationToken token)
        {
            var response = await _itemService.GetAllItems(token);
            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetItem")]
        public async Task<ActionResult<ServiceResponse<GetItemsDto>>> GetItem(string columnName, string value, CancellationToken token)
        {
            var response = await _itemService.GetItemByFilter(columnName, value, token);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}