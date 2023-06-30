using static Goods.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Goods.Services.UserService;


namespace Goods.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetUpDataBaseControler : ControllerBase
    {
        private readonly ISetUpDataBaseService _setUpService;

        public SetUpDataBaseControler(ISetUpDataBaseService setUpService)
        {
            _setUpService = setUpService;
        }

        [HttpPost("SetUp")]
        public async Task<ActionResult<ServiceResponse<string>>> SetUpDataBase(CancellationToken token)
        {
            return Ok(await _setUpService.SetUpDataBase(token));
        }
    }
}