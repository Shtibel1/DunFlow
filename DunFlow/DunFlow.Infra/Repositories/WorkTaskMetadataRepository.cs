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
    public class WorkTaskMetadataRepository : IWorkTaskMetadataRepository
    {
        private readonly ApplicationContext _context;

        public WorkTaskMetadataRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkTaskType>> GetAllTypesWithStatusesAsync()
        {
            return await _context.WorkTaskTypes
                .Include(t => t.Statuses)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<WorkTaskStatus?> GetStatusAsync(int workTaskTypeId, int statusValue)
        {
            return await _context.WorkTaskStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.WorkTaskTypeId == workTaskTypeId && s.StatusValue == statusValue);
        }

        public async Task<IEnumerable<FormField>> GetFormFieldsAsync(int statusId)
        {
            return await _context.FormFields
                .Where(f => f.WorkTaskStatusId == statusId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<FormField>> GetFormFieldsByStatusValueAsync(int workTaskTypeId, int targetStatusValue)
        {
            return await _context.FormFields
                .Where(f => f.WorkTaskStatus.WorkTaskTypeId == workTaskTypeId &&
                            f.WorkTaskStatus.StatusValue == targetStatusValue)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
