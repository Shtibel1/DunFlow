using DunFlow.Domain.Entities;
using DunFlow.Domain.Interfaces;
using DunFlow.Infra.DataContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Infra.Repositories
{
    public class WorkTaskRepository : IWorkTaskRepository
    {
        private readonly ApplicationContext _context;

        public WorkTaskRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<WorkTask?> GetByIdAsync(int id)
        {
            return await _context.WorkTasks
                .Include(t => t.WorkTaskType)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<WorkTask>> GetTasksByUserIdAsync(int userId)
        {
            return await _context.WorkTasks
                .Where(t => t.AssignedUserId == userId)
                .Include(t => t.WorkTaskType)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(WorkTask task)
        {
            await _context.WorkTasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkTask task)
        {
            _context.WorkTasks.Update(task);
            await _context.SaveChangesAsync();
        }
    }
}
