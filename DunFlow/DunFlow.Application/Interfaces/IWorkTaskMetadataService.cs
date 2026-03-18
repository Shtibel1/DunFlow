using DunFlow.Application.Contracts;
using DunFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Interfaces
{
    public interface IWorkTaskMetadataService
    {
        Task<IEnumerable<TaskTypeDto>> GetTaskTypeMetadataAsync();
        Task<IEnumerable<FormFieldDto>> GetFormSchemaAsync(int workTaskTypeId, int targetStatus);
        Task<WorkTaskStatus?> GetStatusEntityAsync(int workTaskTypeId, int statusValue);
        Task<IEnumerable<FormField>> GetRawFormFieldsAsync(int statusId);
    }
}
