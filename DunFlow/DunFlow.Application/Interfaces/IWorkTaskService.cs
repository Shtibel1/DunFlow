using DunFlow.Application.Contracts;
using DunFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Interfaces
{
    public interface IWorkTaskService
    {
        BaseResponse<IEnumerable<TaskTypeOptionDto>> GetSupportedTaskTypes();
        Task<BaseResponse<WorkTaskDetailsDto>> GetByIdAsync(int taskId);
        Task<BaseResponse<CreateTaskResponse>> CreateAsync(CreateTaskRequest request);
        Task<BaseResponse> ChangeStatusAsync(int taskId, ChangeStatusRequest request);
        Task<BaseResponse> CloseAsync(int taskId);
        Task<BaseResponse<IList<WorkTaskDto>>> GetUserTasksAsync(int userId);
        BaseResponse<IEnumerable<FormFieldDto>> GetFormSchema(TaskType type, int targetStatus);
        BaseResponse<IEnumerable<TaskTypeMetadataDto>> GetTaskTypeMetadata();
    }
}
