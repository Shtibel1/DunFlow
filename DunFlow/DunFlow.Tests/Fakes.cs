using DunFlow.Application.Contracts;
using DunFlow.Application.Interfaces;
using DunFlow.Domain.Entities;
using DunFlow.Domain.Interfaces;

namespace DunFlow.Tests;

/// <summary>In-memory repository so the workflow rules can be exercised without a database.</summary>
internal sealed class InMemoryWorkTaskRepository : IWorkTaskRepository
{
    private readonly Dictionary<int, WorkTask> _tasks = new();

    public int UpdateCount { get; private set; }

    public void Seed(WorkTask task) => _tasks[task.Id] = task;

    public Task<WorkTask?> GetByIdAsync(int id) =>
        Task.FromResult(_tasks.TryGetValue(id, out var task) ? task : null);

    public Task<IEnumerable<WorkTask>> GetTasksByUserIdAsync(int userId) =>
        Task.FromResult(_tasks.Values.Where(t => t.AssignedUserId == userId));

    public Task AddAsync(WorkTask task)
    {
        task.Id = _tasks.Count + 1;
        _tasks[task.Id] = task;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(WorkTask task)
    {
        UpdateCount++;
        _tasks[task.Id] = task;
        return Task.CompletedTask;
    }
}

/// <summary>
/// Metadata source describing one task type as a list of sequential statuses,
/// mirroring the WorkTaskTypes / WorkTaskStatuses / FormFields tables.
/// </summary>
internal sealed class FakeWorkflowMetadata : IWorkTaskMetadataService
{
    private readonly List<WorkTaskStatus> _statuses = new();

    public FakeWorkflowMetadata AddStatus(int typeId, int value, bool isFinal = false, params FormField[] fields)
    {
        var status = new WorkTaskStatus
        {
            Id = _statuses.Count + 100,
            WorkTaskTypeId = typeId,
            StatusValue = value,
            DisplayName = $"Status {value}",
            IsFinal = isFinal,
        };
        foreach (var field in fields)
        {
            field.WorkTaskStatusId = status.Id;
            status.FormFields.Add(field);
        }
        _statuses.Add(status);
        return this;
    }

    public Task<WorkTaskStatus?> GetStatusEntityAsync(int workTaskTypeId, int statusValue) =>
        Task.FromResult(_statuses.FirstOrDefault(s => s.WorkTaskTypeId == workTaskTypeId && s.StatusValue == statusValue));

    public Task<IEnumerable<FormField>> GetRawFormFieldsAsync(int statusId) =>
        Task.FromResult(_statuses.Where(s => s.Id == statusId).SelectMany(s => s.FormFields));

    public Task<IEnumerable<TaskTypeDto>> GetTaskTypeMetadataAsync() => throw new NotSupportedException();

    public Task<IEnumerable<FormFieldDto>> GetFormSchemaAsync(int workTaskTypeId, int targetStatus) => throw new NotSupportedException();

    public static FormField Required(string key, string label) =>
        new() { Key = key, Label = label, ControlType = "text", IsRequired = true };

    public static FormField Optional(string key, string label) =>
        new() { Key = key, Label = label, ControlType = "text", IsRequired = false };
}
