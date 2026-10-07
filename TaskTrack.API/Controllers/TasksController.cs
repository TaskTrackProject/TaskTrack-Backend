using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("project/{projectId:int}")]
    public async Task<IActionResult> GetByProject(
        int projectId)
    {
        var result =
            await _service.GetByProjectAsync(projectId);

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? title,
        [FromQuery] int? status,
        [FromQuery] int? priority,
        [FromQuery] int? projectId)
    {
        var result = await _service.SearchAsync(
            title,
            status,
            priority,
            projectId);

        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromBody] TaskRequest request)
    {
        if (!int.TryParse(User.FindFirst("AccountID")?.Value, out var accountId))
            return Unauthorized();

        var result = await _service.CreateAsync(request, accountId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.TaskId },
            result);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] TaskRequest request)
    {
        var result =
            await _service.UpdateAsync(id, request);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }
}