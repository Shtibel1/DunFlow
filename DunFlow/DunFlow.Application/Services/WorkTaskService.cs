using AutoMapper;
using DunFlow.Application.Contracts;
using DunFlow.Application.Interfaces;
using DunFlow.Domain.Entities;
using DunFlow.Domain.Enums;
using DunFlow.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DunFlow.Application.Services
{
    public class WorkTaskService : IWorkTaskService
    {
        private readonly IMapper _mapper;
        private readonly IWorkTaskRepository _repository;
        private readonly ITaskStrategyResolver _strategyResolver;

        public WorkTaskService(IMapper mapper, IWorkTaskRepository repository, ITaskStrategyResolver strategyResolver)
        {
            _mapper = mapper;
            _repository = repository;
            _strategyResolver = strategyResolver;
        }

        public BaseResponse<IEnumerable<TaskTypeOptionDto>> GetSupportedTaskTypes()
        {
            var types = _strategyResolver
                .GetSupportedTaskTypes()
                .Select(type => new TaskTypeOptionDto
                {
                    Value = (int)type,
                    Label = type.ToString()
                });

            return BaseResponse<IEnumerable<TaskTypeOptionDto>>.Success(types);
        }

        public async Task<BaseResponse<CreateTaskResponse>> CreateAsync(CreateTaskRequest request)
        {
            try
            {
                _strategyResolver.GetStrategy(request.Type);
            }
            catch (NotSupportedException ex)
            {
                return BaseResponse<CreateTaskResponse>.Failure(ex.Message);
            }

            var newTask = _mapper.Map<WorkTask>(request);

            await _repository.AddAsync(newTask);

            return BaseResponse<CreateTaskResponse>.Success(new CreateTaskResponse { TaskId = newTask.Id});
        }

        public async Task<BaseResponse> ChangeStatusAsync(int taskId, ChangeStatusRequest request)
        {
            var task = await _repository.GetByIdAsync(taskId);

            var preCheck = ValidateBasicState(task);
            if (!preCheck.IsSuccess) return preCheck;

            var strategy = _strategyResolver.GetStrategy(task.Type);

            var transitionCheck = ValidateTransition(task, request.TargetStatus, strategy);
            if (!transitionCheck.IsSuccess) return transitionCheck;

            if (!strategy.ValidateRequiredData(request.TargetStatus, request.CustomFieldsJson, out var errorMessage))
            {
                return BaseResponse.Failure(errorMessage);
            }

            UpdateTaskState(task, request);
            await _repository.UpdateAsync(task);

            return BaseResponse.Success();
        }

        private BaseResponse ValidateBasicState(WorkTask task)
        {
            if (task == null) return BaseResponse.Failure("Task not found.");
            if (task.IsClosed) return BaseResponse.Failure("Cannot change the status of a closed task.");
            return BaseResponse.Success();
        }

        private BaseResponse ValidateTransition(WorkTask task, int targetStatus, ITaskTypeStrategy strategy)
        {
            if (targetStatus == task.CurrentStatus)
                return BaseResponse.Failure("Target status is the same as the current status.");

            if (targetStatus > task.CurrentStatus)
            {
                if (targetStatus != task.CurrentStatus + 1)
                    return BaseResponse.Failure("Forward moves must be sequential (no skipping allowed).");

                if (targetStatus > strategy.FinalStatus)
                    return BaseResponse.Failure($"Target status exceeds the final status ({strategy.FinalStatus}) for this task type.");
            }

            return BaseResponse.Success();
        }

        private void UpdateTaskState(WorkTask task, ChangeStatusRequest request)
        {
            task.CurrentStatus = request.TargetStatus;
            task.AssignedUserId = request.NextAssignedUserId;
            task.CustomFieldsJson = request.CustomFieldsJson;
            task.UpdatedAt = DateTime.UtcNow;
        }

        public async Task<BaseResponse> CloseAsync(int taskId)
        {
            var task = await _repository.GetByIdAsync(taskId);

            if (task == null)
                return BaseResponse.Failure("Task not found.");

            if (task.IsClosed)
                return BaseResponse.Failure("Task is already closed.");

            var strategy = _strategyResolver.GetStrategy(task.Type);

            if (task.CurrentStatus != strategy.FinalStatus)
                return BaseResponse.Failure($"A task may be closed only at its final status ({strategy.FinalStatus}).");

            task.IsClosed = true;
            task.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(task);

            return BaseResponse.Success();
        }

        public async Task<BaseResponse<IList<WorkTaskDto>>> GetUserTasksAsync(int userId)
        {
            var tasks = await _repository.GetTasksByUserIdAsync(userId);
            var dtos = _mapper.Map<IList<WorkTaskDto>>(tasks);

            return BaseResponse<IList<WorkTaskDto>>.Success(dtos);
        }

        public async Task<BaseResponse<WorkTaskDetailsDto>> GetByIdAsync(int taskId)
        {
            var task = await _repository.GetByIdAsync(taskId);

            if (task == null)
                return BaseResponse<WorkTaskDetailsDto>.Failure("Task not found.");

            var strategy = _strategyResolver.GetStrategy(task.Type);
            var dto = _mapper.Map<WorkTaskDetailsDto>(task);
            dto.CanBeClosed = (task.CurrentStatus == strategy.FinalStatus && !task.IsClosed);
            return BaseResponse<WorkTaskDetailsDto>.Success(dto);
        }

        public BaseResponse<IEnumerable<FormFieldDto>> GetFormSchema(TaskType type, int targetStatus)
        {
            var strategy = _strategyResolver.GetStrategy(type);

            var schemaDefinitions = strategy.GetFormSchema(targetStatus);

            var dtos = _mapper.Map<IEnumerable<FormFieldDto>>(schemaDefinitions);

            return BaseResponse<IEnumerable<FormFieldDto>>.Success(dtos);
        }

        public BaseResponse<IEnumerable<TaskTypeMetadataDto>> GetTaskTypeMetadata()
        {
            var metadata = _strategyResolver.GetAll().Select(s => new TaskTypeMetadataDto
            {
                TypeValue = (int)s.TaskType,
                TaskType = s.TaskType.ToString(),
                FinalStatus = s.FinalStatus,
                Statuses = s.GetStatusNames()
            });

            return BaseResponse<IEnumerable<TaskTypeMetadataDto>>.Success(metadata);
        }

    }
}
