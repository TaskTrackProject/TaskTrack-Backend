using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ITaskService _taskService;

    public SearchController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? title,
        [FromQuery] int? status,
        [FromQuery] int? priority,
        [FromQuery] int? projectId)
    {
        return Ok(await _taskService.SearchAsync(title, status, priority, projectId));
    }
}