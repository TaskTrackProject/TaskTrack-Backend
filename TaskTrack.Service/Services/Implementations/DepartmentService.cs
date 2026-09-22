using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Repo.DTOs.Responses;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Implementations;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;

    public DepartmentService(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DepartmentResponse>> GetAllAsync()
    {
        var departments = await _repository.GetAllActiveAsync();

        return departments
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<DepartmentDetailResponse?> GetByIdAsync(int id)
    {
        ValidateId(id);

        var department = await _repository.GetByIdAsync(id);

        if (department == null)
            return null;

        return new DepartmentDetailResponse
        {
            DepartmentId = department.DepartmentId,
            DepartmentName = department.DepartmentName,
            DepartmentDescription = department.DepartmentDescription,
            IsActive = department.IsActive,

            Projects = department.Projects
                .Select(p => new ProjectResponse
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    Status = p.Status,
                    DepartmentId = p.DepartmentId,
                    IsActive = p.IsActive
                })
                .ToList()
        };
    }

    public async Task<List<DepartmentResponse>> SearchAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new List<DepartmentResponse>();

        name = name.Trim();

        var departments = await _repository.SearchByNameAsync(name);

        return departments
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<DepartmentResponse> CreateAsync(
        DepartmentRequest request)
    {
        ValidateRequest(request);

        var department = new Department
        {
            DepartmentName = request.DepartmentName.Trim(),
            DepartmentDescription = request.DepartmentDescription?.Trim(),
            IsActive = true
        };

        await _repository.AddAsync(department);

        return MapToResponse(department);
    }

    public async Task<DepartmentResponse?> UpdateAsync(
        int id,
        DepartmentRequest request)
    {
        ValidateId(id);
        ValidateRequest(request);

        var department = await _repository.GetByIdAsync(id);

        if (department == null)
            return null;

        department.DepartmentName = request.DepartmentName.Trim();

        department.DepartmentDescription =
            request.DepartmentDescription?.Trim();

        await _repository.UpdateAsync(department);

        return MapToResponse(department);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        ValidateId(id);

        var department = await _repository.GetByIdAsync(id);

        if (department == null)
            return false;

        var hasProjects = await _repository.HasProjectsAsync(id);

        if (hasProjects)
        {
            throw new InvalidOperationException(
                "Cannot delete department because it has projects.");
        }

        await _repository.DeleteAsync(department);

        return true;
    }

    private static DepartmentResponse MapToResponse(
        Department department)
    {
        return new DepartmentResponse
        {
            DepartmentId = department.DepartmentId,
            DepartmentName = department.DepartmentName,
            DepartmentDescription = department.DepartmentDescription,
            IsActive = department.IsActive
        };
    }

    private static void ValidateRequest(
        DepartmentRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.DepartmentName))
            throw new ArgumentException(
                "Department name is required.");
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ArgumentException(
                "Department ID must be greater than 0.");
    }
}