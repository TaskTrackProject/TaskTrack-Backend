using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Repo.DTOs.Responses;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.Helpers;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Implementations;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly TaskValidator _validator;

    public TaskService(
        ITaskRepository repository,
        TaskValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<List<TaskResponse>> GetAllAsync()
    {
        var tasks = await _repository.GetAllActiveAsync();

        return tasks
            .Select(TaskMapper.ToResponse)
            .ToList();
    }

    public async Task<TaskResponse?> GetByIdAsync(int id)
    {
        _validator.ValidateId(id);

        var task = await _repository.GetByIdAsync(id);

        if (task == null || !task.IsActive)
            return null;

        return TaskMapper.ToResponse(task);
    }

    public async Task<List<TaskResponse>> GetByProjectAsync(
        int projectId)
    {
        _validator.ValidateId(projectId);

        var projectExists =
            await _repository.ProjectExistsAsync(projectId);

        if (!projectExists)
        {
            throw new KeyNotFoundException(
                "Project not found.");
        }

        var tasks =
            await _repository.GetByProjectAsync(projectId);

        return tasks
            .Select(TaskMapper.ToResponse)
            .ToList();
    }

    public async Task<List<TaskResponse>> SearchAsync(
        string? title,
        int? status,
        int? priority,
        int? projectId)
    {
        _validator.ValidateStatus(status);
        _validator.ValidatePriority(priority);

        if (projectId.HasValue)
        {
            if (projectId.Value <= 0)
            {
                throw new ArgumentException(
                    "Project ID must be greater than 0.");
            }

            var projectExists =
                await _repository.ProjectExistsAsync(
                    projectId.Value);

            if (!projectExists)
            {
                throw new KeyNotFoundException(
                    "Project not found.");
            }
        }

        title = title?.Trim();

        var tasks = await _repository.SearchAsync(
            title,
            status,
            priority,
            projectId);

        return tasks
            .Select(TaskMapper.ToResponse)
            .ToList();
    }

    public async Task<TaskResponse> CreateAsync(
        TaskRequest request)
    {
        _validator.ValidateRequest(request);

        var projectExists =
            await _repository.ProjectExistsAsync(
                request.ProjectId);

        if (!projectExists)
        {
            throw new KeyNotFoundException(
                "Project not found.");
        }

        var task = new Repo.Models.Task
        {
            Title = request.Title.Trim(),

            Description = string.IsNullOrWhiteSpace(
                request.Description)
                ? null
                : request.Description.Trim(),

            Status = (short)request.Status,

            Priority = (short)request.Priority,

            DueDate = request.DueDate,

            ProjectId = request.ProjectId,

            IsActive = true,

            CreatedDate = DateTime.Now,

            ModifiedDate = null
        };

        await _repository.AddAsync(task);
        await _repository.SetTagsAsync(task, request.TagIds);

        return TaskMapper.ToResponse(task);
    }

    public async Task<TaskResponse?> UpdateAsync(
        int id,
        TaskRequest request)
    {
        _validator.ValidateId(id);
        _validator.ValidateRequest(request);

        var task = await _repository.GetByIdAsync(id);

        if (task == null || !task.IsActive)
            return null;

        var projectExists =
            await _repository.ProjectExistsAsync(
                request.ProjectId);

        if (!projectExists)
        {
            throw new KeyNotFoundException(
                "Project not found.");
        }

        task.Title = request.Title.Trim();

        task.Description =
            string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();

        task.Status = (short)request.Status;

        task.Priority = (short)request.Priority;

        task.DueDate = request.DueDate;

        task.ProjectId = request.ProjectId;

        task.ModifiedDate = DateTime.Now;

        await _repository.UpdateAsync(task);
        await _repository.SetTagsAsync(task, request.TagIds);

        return TaskMapper.ToResponse(task);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _validator.ValidateId(id);

        var task = await _repository.GetByIdAsync(id);

        if (task == null || !task.IsActive)
            return false;

        await _repository.DeleteAsync(task);

        return true;
    }
}