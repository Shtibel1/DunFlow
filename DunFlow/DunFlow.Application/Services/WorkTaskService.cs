using AutoMapper;
using DunFlow.Application.Contracts;
using DunFlow.Application.Interfaces;
using DunFlow.Domain.Entities;
using DunFlow.Domain.Interfaces;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace DunFlow.Application.Services
{
    public class WorkTaskService : IWorkTaskService
    {
        private readonly IWorkTaskRepository _repository;
        private readonly IWorkTaskMetadataService _metadataService;
        private readonly IMapper _mapper;

        public WorkTaskService(
            IWorkTaskRepository repository,
            IWorkTaskMetadataService metadataService,
            IMapper mapper)
        {
            _repository = repository;
            _metadataService = metadataService;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CreateTaskRequest request)
        {
            var newTask = _mapper.Map<WorkTask>(request);
            newTask.CurrentStatus = 1;
            await _repository.AddAsync(newTask);
            return newTask.Id;
        }

        public async Task ChangeStatusAsync(int taskId, ChangeStatusRequest request)
        {
            var task = await _repository.GetByIdAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException("Task not found.");

            if (task.IsClosed)
                throw new InvalidOperationException("Cannot change status of a closed task.");

            var isStatusChange = request.TargetStatus != task.CurrentStatus;

            if (isStatusChange && request.TargetStatus > task.CurrentStatus && request.TargetStatus != task.CurrentStatus + 1)
                throw new InvalidOperationException("Forward moves must be sequential.");

            var targetStatusEntity = await _metadataService.GetStatusEntityAsync(task.WorkTaskTypeId, request.TargetStatus);
            if (targetStatusEntity == null)
                throw new InvalidOperationException("Invalid target status.");

            var formFields = await _metadataService.GetRawFormFieldsAsync(targetStatusEntity.Id);
            if (!ValidateStructuralData(request.CustomFieldsJson, formFields, out var structError))
                throw new ArgumentException(structError);

            task.CurrentStatus = request.TargetStatus;
            task.AssignedUserId = request.NextAssignedUserId;
            task.UpdatedAt = DateTime.UtcNow;

            task.CustomFieldsJson = MergeJsonData(task.CustomFieldsJson, request.CustomFieldsJson);

            await _repository.UpdateAsync(task);
        }

        public async Task CloseAsync(int taskId)
        {
            var task = await _repository.GetByIdAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException("Task not found.");

            if (task.IsClosed)
                throw new InvalidOperationException("Task already closed.");

            var statusEntity = await _metadataService.GetStatusEntityAsync(task.WorkTaskTypeId, task.CurrentStatus);
            if (statusEntity == null || !statusEntity.IsFinal)
                throw new InvalidOperationException("Task can only be closed at final status.");

            task.IsClosed = true;
            task.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(task);
        }

        public async Task<WorkTaskDetailsDto> GetByIdAsync(int taskId)
        {
            var task = await _repository.GetByIdAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException("Task not found.");

            var statusEntity = await _metadataService.GetStatusEntityAsync(task.WorkTaskTypeId, task.CurrentStatus);

            var dto = _mapper.Map<WorkTaskDetailsDto>(task);
            dto.CanBeClosed = (statusEntity?.IsFinal ?? false) && !task.IsClosed;

            return dto;
        }

        public async Task<IList<WorkTaskDto>> GetUserTasksAsync(int userId)
        {
            var tasks = await _repository.GetTasksByUserIdAsync(userId);
            return _mapper.Map<IList<WorkTaskDto>>(tasks);
        }

        private bool ValidateStructuralData(string? json, IEnumerable<FormField> fields, out string error)
        {
            error = string.Empty;
            var requiredFields = fields.Where(f => f.IsRequired).ToList();
            if (!requiredFields.Any()) return true;

            using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
            foreach (var field in requiredFields)
            {
                if (!doc.RootElement.TryGetProperty(field.Key, out var prop) || string.IsNullOrWhiteSpace(prop.ToString()))
                {
                    error = $"'{field.Label}' is required.";
                    return false;
                }
            }
            return true;
        }

        private string MergeJsonData(string existingJson, string? newJson)
        {
            if (string.IsNullOrWhiteSpace(newJson) || newJson == "{}") return existingJson;
            if (string.IsNullOrWhiteSpace(existingJson) || existingJson == "{}") return newJson;

            try
            {
                var existingDict = JsonSerializer.Deserialize<Dictionary<string, object>>(existingJson) ?? new();
                var newDict = JsonSerializer.Deserialize<Dictionary<string, object>>(newJson) ?? new();

                foreach (var item in newDict)
                {
                    existingDict[item.Key] = item.Value;
                }

                return JsonSerializer.Serialize(existingDict);
            }
            catch
            {
                return newJson;
            }
        }
    }
}
