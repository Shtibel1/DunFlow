using DunFlow.Application.Contracts;
using DunFlow.Application.Interfaces;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        {
            var id = await _taskService.CreateAsync(request);
            return CreatedAtAction(nameof(GetTask), new { id }, new { TaskId = id });
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusRequest request)
        {
            await _taskService.ChangeStatusAsync(id, request);
            return NoContent();
        }

        [HttpPut("{id}/close")]
        public async Task<IActionResult> Close(int id)
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
    }
}
