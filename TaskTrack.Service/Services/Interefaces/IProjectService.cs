using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Repo.DTOs.Responses;

namespace TaskTrack.Service.Interfaces;

public interface IProjectService
{
    Task<List<ProjectResponse>> GetAllAsync();

    Task<ProjectDetailResponse?> GetByIdAsync(int id);

    Task<List<ProjectResponse>> GetByDepartmentAsync(
        int departmentId);

    Task<List<ProjectResponse>> SearchAsync(
        string? name,
        int? status,
        int? departmentId);

    Task<ProjectResponse> CreateAsync(ProjectRequest request);

    Task<ProjectResponse?> UpdateAsync(
        int id,
        ProjectRequest request);

    Task<bool> DeleteAsync(int id);
}