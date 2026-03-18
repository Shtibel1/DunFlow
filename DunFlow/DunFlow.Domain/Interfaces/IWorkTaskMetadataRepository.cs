using DunFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Domain.Interfaces
{
    public interface IWorkTaskMetadataRepository
    {
        Task<IEnumerable<WorkTaskType>> GetAllTypesWithStatusesAsync();
        Task<WorkTaskStatus?> GetStatusAsync(int workTaskTypeId, int statusValue);
        Task<IEnumerable<FormField>> GetFormFieldsAsync(int statusId);
        Task<IEnumerable<FormField>> GetFormFieldsByStatusValueAsync(int workTaskTypeId, int targetStatusValue);
    }
}
