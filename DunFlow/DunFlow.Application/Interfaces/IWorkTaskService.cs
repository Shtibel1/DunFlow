using DunFlow.Application.Contracts;

namespace DunFlow.Application.Interfaces
{
    public interface IWorkTaskService
    {
        Task<WorkTaskDetailsDto> GetByIdAsync(int taskId);
        Task<int> CreateAsync(CreateTaskRequest request);
        Task ChangeStatusAsync(int taskId, ChangeStatusRequest request);
        Task CloseAsync(int taskId);
        Task<IList<WorkTaskDto>> GetUserTasksAsync(int userId);
    }
}
