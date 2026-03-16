using DunFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllAsync();
    }
}
