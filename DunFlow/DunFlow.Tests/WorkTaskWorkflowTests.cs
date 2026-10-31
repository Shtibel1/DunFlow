using System.Text.Json;
using DunFlow.Application.Contracts;
using DunFlow.Application.Services;
using DunFlow.Domain.Entities;

namespace DunFlow.Tests;

/// <summary>
/// Behavioural tests for the metadata-driven state machine in <see cref="WorkTaskService"/>:
/// sequential forward moves, free backward moves, per-status required fields,
/// custom-field merging, and closing only at a final status.
/// </summary>
public class WorkTaskWorkflowTests
{
    private const int TypeId = 1;

    private readonly InMemoryWorkTaskRepository _repository = new();
    private readonly FakeWorkflowMetadata _metadata = new FakeWorkflowMetadata()
        .AddStatus(TypeId, 1)
        .AddStatus(TypeId, 2, fields: new[] { FakeWorkflowMetadata.Required("specLink", "Spec Link") })
        .AddStatus(TypeId, 3, fields: new[]
        {
            FakeWorkflowMetadata.Required("branchName", "Branch Name"),
            FakeWorkflowMetadata.Optional("notes", "Notes"),
        })
        .AddStatus(TypeId, 4, isFinal: true);

    // The mapper is only used by create/read paths, which these tests don't touch.
    private WorkTaskService CreateService() => new(_repository, _metadata, mapper: null!);

    private WorkTask SeedTask(int status = 1, bool closed = false, string customFields = "{}")
    {
        var task = new WorkTask
        {
            Id = 42,
            Title = "Ship the workflow engine",
            WorkTaskTypeId = TypeId,
            CurrentStatus = status,
            IsClosed = closed,
            AssignedUserId = 7,
            CustomFieldsJson = customFields,
        };
        _repository.Seed(task);
        return task;
    }

    private static ChangeStatusRequest Move(int target, object? fields = null, int assignee = 8) => new()
    {
        TargetStatus = target,
        NextAssignedUserId = assignee,
        CustomFieldsJson = fields is null ? null : JsonSerializer.Serialize(fields),
    };

    [Fact]
    public async Task Moving_forward_one_step_with_required_data_updates_status_and_assignee()
    {
        var task = SeedTask(status: 1);
        var before = DateTime.UtcNow;

        await CreateService().ChangeStatusAsync(task.Id, Move(2, new { specLink = "https://example.com/spec" }, assignee: 9));

        Assert.Equal(2, task.CurrentStatus);
        Assert.Equal(9, task.AssignedUserId);
        Assert.NotNull(task.UpdatedAt);
        Assert.True(task.UpdatedAt >= before);
        Assert.Equal(1, _repository.UpdateCount);
    }

    [Fact]
    public async Task Skipping_a_status_forward_is_rejected()
    {
        var task = SeedTask(status: 1);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateService().ChangeStatusAsync(task.Id, Move(3, new { branchName = "feature/x" })));

        Assert.Equal("Forward moves must be sequential.", ex.Message);
        Assert.Equal(1, task.CurrentStatus);
        Assert.Equal(0, _repository.UpdateCount);
    }

    [Fact]
    public async Task Moving_backward_to_any_earlier_status_is_allowed()
    {
        var task = SeedTask(status: 3);

        await CreateService().ChangeStatusAsync(task.Id, Move(1));

        Assert.Equal(1, task.CurrentStatus);
    }

    [Fact]
    public async Task Staying_on_the_same_status_reassigns_without_moving()
    {
        var task = SeedTask(status: 2, customFields: """{"specLink":"a"}""");

        await CreateService().ChangeStatusAsync(task.Id, Move(2, new { specLink = "b" }, assignee: 11));

        Assert.Equal(2, task.CurrentStatus);
        Assert.Equal(11, task.AssignedUserId);
    }

    [Fact]
    public async Task Missing_required_field_for_target_status_is_rejected_with_its_label()
    {
        var task = SeedTask(status: 1);

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => CreateService().ChangeStatusAsync(task.Id, Move(2, new { unrelated = "x" })));

        Assert.Contains("Spec Link", ex.Message);
        Assert.Equal(1, task.CurrentStatus);
    }

    [Fact]
    public async Task Blank_required_field_counts_as_missing()
    {
        var task = SeedTask(status: 1);

        await Assert.ThrowsAsync<ArgumentException>(
            () => CreateService().ChangeStatusAsync(task.Id, Move(2, new { specLink = "   " })));
    }

    [Fact]
    public async Task No_payload_is_rejected_when_the_target_status_has_required_fields()
    {
        var task = SeedTask(status: 1);

        await Assert.ThrowsAsync<ArgumentException>(
            () => CreateService().ChangeStatusAsync(task.Id, Move(2)));
    }

    [Fact]
    public async Task Status_not_defined_for_the_task_type_is_rejected()
    {
        var task = SeedTask(status: 4);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateService().ChangeStatusAsync(task.Id, Move(5)));

        Assert.Equal("Invalid target status.", ex.Message);
    }

    [Fact]
    public async Task Closed_tasks_cannot_change_status()
    {
        var task = SeedTask(status: 4, closed: true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateService().ChangeStatusAsync(task.Id, Move(3, new { branchName = "x" })));
    }

    [Fact]
    public async Task Unknown_task_is_reported_as_not_found()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => CreateService().ChangeStatusAsync(999, Move(2)));
    }

    [Fact]
    public async Task Custom_fields_are_merged_and_new_values_win()
    {
        var task = SeedTask(status: 2, customFields: """{"specLink":"old","reviewer":"dana"}""");

        await CreateService().ChangeStatusAsync(task.Id, Move(3, new { branchName = "feature/engine", specLink = "new" }));

        using var merged = JsonDocument.Parse(task.CustomFieldsJson);
        Assert.Equal("new", merged.RootElement.GetProperty("specLink").GetString());
        Assert.Equal("dana", merged.RootElement.GetProperty("reviewer").GetString());
        Assert.Equal("feature/engine", merged.RootElement.GetProperty("branchName").GetString());
    }

    [Fact]
    public async Task Empty_payload_keeps_existing_custom_fields()
    {
        const string existing = """{"specLink":"keep-me"}""";
        var task = SeedTask(status: 3, customFields: existing);

        await CreateService().ChangeStatusAsync(task.Id, Move(4, new { }));

        Assert.Equal(existing, task.CustomFieldsJson);
        Assert.Equal(4, task.CurrentStatus);
    }

    [Fact]
    public async Task Task_can_be_closed_only_at_a_final_status()
    {
        var task = SeedTask(status: 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => CreateService().CloseAsync(task.Id));

        Assert.Equal("Task can only be closed at final status.", ex.Message);
        Assert.False(task.IsClosed);
    }

    [Fact]
    public async Task Closing_at_final_status_marks_the_task_closed()
    {
        var task = SeedTask(status: 4);

        await CreateService().CloseAsync(task.Id);

        Assert.True(task.IsClosed);
        Assert.NotNull(task.UpdatedAt);
        Assert.Equal(1, _repository.UpdateCount);
    }

    [Fact]
    public async Task Closing_twice_is_rejected()
    {
        var task = SeedTask(status: 4, closed: true);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => CreateService().CloseAsync(task.Id));

        Assert.Equal("Task already closed.", ex.Message);
    }
}
