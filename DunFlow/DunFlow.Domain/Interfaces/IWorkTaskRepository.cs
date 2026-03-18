using DunFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Domain.Interfaces
{
    public interface IWorkTaskRepository
    {
        Task<WorkTask?> GetByIdAsync(int id);
        Task<IEnumerable<WorkTask>> GetTasksByUserIdAsync(int userId);
        Task AddAsync(WorkTask task);
        Task UpdateAsync(WorkTask task);
    }
}
