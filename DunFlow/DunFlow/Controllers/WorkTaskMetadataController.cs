using DunFlow.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DunFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkTaskMetadataController : ControllerBase
    {
        private readonly IWorkTaskMetadataService _metadataService;

        public WorkTaskMetadataController(IWorkTaskMetadataService metadataService)
        {
            _metadataService = metadataService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMetadata()
        {
            var metadata = await _metadataService.GetTaskTypeMetadataAsync();
            return Ok(metadata);
        }

        [HttpGet("schema/{workTaskTypeId}/{targetStatus}")]
        public async Task<IActionResult> GetSchema(int workTaskTypeId, int targetStatus)
        {
            var schema = await _metadataService.GetFormSchemaAsync(workTaskTypeId, targetStatus);
            return Ok(schema);
        }
    }
}
