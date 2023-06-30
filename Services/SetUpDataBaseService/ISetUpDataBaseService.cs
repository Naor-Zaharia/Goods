using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goods.Services.UserService
{
    public interface ISetUpDataBaseService
    {
        Task<ServiceResponse<string>> SetUpDataBase(CancellationToken token);
    }
}