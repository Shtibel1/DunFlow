using DunFlow.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<IEnumerable<UserDto>>> GetAllUsersAsync();
    }
}
