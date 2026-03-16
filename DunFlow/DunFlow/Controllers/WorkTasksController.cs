using DunFlow.Application.Contracts;
using DunFlow.Application.Interfaces;
using DunFlow.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DunFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkTasksController : ControllerBase
    {
        private readonly IWorkTaskService _taskService;

        public WorkTasksController(IWorkTaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("types")]
        public IActionResult GetTaskTypes()
        {
            var response = _taskService.GetSupportedTaskTypes();

            if (!response.IsSuccess)
            {
                return BadRequest(new { Error = response.ErrorMessage });
            }

            return Ok(response.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var response = await _taskService.GetByIdAsync(id);

            if (!response.IsSuccess)
            {
                return NotFound(new { Error = response.ErrorMessage });
            }

            return Ok(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
        {
            var taskId = await _taskService.CreateAsync(request);
            return CreatedAtAction(nameof(GetUserTasks), new { userId = request.AssignedUserId }, new { TaskId = taskId });
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusRequest request)
        {
            await _taskService.ChangeStatusAsync(id, request);
            return NoContent();
        }

        [HttpPut("{id}/close")]
        public async Task<IActionResult> CloseTask(int id)
        {
            await _taskService.CloseAsync(id);
            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserTasks(int userId)
        {
            var tasks = await _taskService.GetUserTasksAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("schema/{type}/{targetStatus}")]
        public IActionResult GetFormSchema(TaskType type, int targetStatus)
        {
            var response = _taskService.GetFormSchema(type, targetStatus);

            if (!response.IsSuccess)
                return BadRequest(new { Error = response.ErrorMessage });

            return Ok(response.Data);
        }

        [HttpGet("metadata")]
        public IActionResult GetTaskTypeMetadata()
        {
            var response = _taskService.GetTaskTypeMetadata();

            if (!response.IsSuccess)
            {
                return BadRequest(new { Error = response.ErrorMessage });
            }

            return Ok(response.Data);
        }
    }
}
