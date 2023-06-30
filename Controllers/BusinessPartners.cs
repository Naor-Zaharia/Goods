using static Goods.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Goods.Services.BusinessPartnersService;
using Goods.Dtos.BusinessPartners;

namespace Goods.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessPartnersControler : ControllerBase
    {
        private readonly IBusinessPartnersService _businessPartnersService;

        public BusinessPartnersControler(IBusinessPartnersService businessPartnersService)
        {
            _businessPartnersService = businessPartnersService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetBusinessPartnersDto>>>> Get(CancellationToken token)
        {
            var response = await _businessPartnersService.GetAllBusinessPartners(token);
            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetBusinessPartners")]
        public async Task<ActionResult<ServiceResponse<GetBusinessPartnersDto>>> GetBusinessPartners(string columnName, string value, CancellationToken token)
        {
            var response = await _businessPartnersService.GetBusinessPartnersByFilter(columnName, value, token);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
