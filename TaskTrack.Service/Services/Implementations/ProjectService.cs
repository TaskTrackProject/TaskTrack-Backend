using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Repo.DTOs.Responses;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.Helpers;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Implementations;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;
    private readonly ProjectValidator _validator;

    public ProjectService(IProjectRepository repository, ProjectValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<List<ProjectResponse>> GetAllAsync()
    {
        var projects = await _repository.GetAllActiveAsync();

        return projects
            .Select(ProjectMapper.ToResponse)
            .ToList();
    }

    public async Task<ProjectDetailResponse?> GetByIdAsync(int id)
    {
        _validator.ValidateId(id);

        var project = await _repository.GetByIdAsync(id);

        if (project == null || !project.IsActive)
            return null;

        return ProjectMapper.ToDetailResponse(project);
    }

    public async Task<List<ProjectResponse>> GetByDepartmentAsync(
        int departmentId)
    {
        _validator.ValidateId(departmentId);

        var departmentExists =
            await _repository.DepartmentExistsAsync(departmentId);

        if (!departmentExists)
            throw new KeyNotFoundException(
                "Department not found.");

        var projects =
            await _repository.GetByDepartmentAsync(departmentId);

        return projects
            .Select(ProjectMapper.ToResponse)
            .ToList();
    }

    public async Task<List<ProjectResponse>> SearchAsync(
        string? name,
        int? status,
        int? departmentId)
    {
        _validator.ValidateStatus(status);

        if (departmentId.HasValue)
        {
            _validator.ValidateId(departmentId.Value);

            var departmentExists =
                await _repository.DepartmentExistsAsync(
                    departmentId.Value);

            if (!departmentExists)
                throw new KeyNotFoundException(
                    "Department not found.");
        }

        name = name?.Trim();

        var projects = await _repository.SearchAsync(
            name,
            status,
            departmentId);

        return projects
            .Select(ProjectMapper.ToResponse)
            .ToList();
    }

    public async Task<ProjectResponse> CreateAsync(
    ProjectRequest request)
    {
        _validator.ValidateRequest(request);

        var departmentExists =
            await _repository.DepartmentExistsAsync(
                request.DepartmentId);

        if (!departmentExists)
        {
            throw new KeyNotFoundException(
                "Department not found.");
        }

        var project = new Project
        {
            ProjectName = request.ProjectName.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = (short)request.Status,
            DepartmentId = request.DepartmentId,
            IsActive = true,
            CreatedDate = DateTime.Now
        };

        await _repository.AddAsync(project);

        return ProjectMapper.ToResponse(project);
    }

    public async Task<ProjectResponse?> UpdateAsync(int id, ProjectRequest request)
    {
        _validator.ValidateId(id);
        _validator.ValidateRequest(request);

        var project = await _repository.GetByIdAsync(id);

        if (project == null || !project.IsActive)
            return null;

        var departmentExists =
            await _repository.DepartmentExistsAsync(
                request.DepartmentId);

        if (!departmentExists)
        {
            throw new KeyNotFoundException(
                "Department not found.");
        }

        project.ProjectName = request.ProjectName.Trim();
        project.Description = string.IsNullOrWhiteSpace(request.Description)
            ? null
            : request.Description.Trim();
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;
        project.Status = (short)request.Status;
        project.DepartmentId = request.DepartmentId;

        await _repository.UpdateAsync(project);

        return ProjectMapper.ToResponse(project);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _validator.ValidateId(id);

        var project = await _repository.GetByIdAsync(id);

        if (project == null || !project.IsActive)
            return false;

        if (await _repository.HasTasksAsync(id))
        {
            throw new InvalidOperationException(
                "Cannot delete project because it has tasks.");
        }

        await _repository.DeleteAsync(project);

        return true;
    }
}

