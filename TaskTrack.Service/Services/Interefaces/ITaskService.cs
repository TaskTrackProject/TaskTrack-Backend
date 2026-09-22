using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Repo.DTOs.Responses;

namespace TaskTrack.Service.Interfaces;

public interface ITaskService
{
    Task<List<TaskResponse>> GetAllAsync();

    Task<TaskResponse?> GetByIdAsync(int id);

    Task<List<TaskResponse>> GetByProjectAsync(
        int projectId);

    Task<List<TaskResponse>> SearchAsync(
        string? title,
        int? status,
        int? priority,
        int? projectId);

    Task<TaskResponse> CreateAsync(TaskRequest request);

    Task<TaskResponse?> UpdateAsync(
        int id,
        TaskRequest request);

    Task<bool> DeleteAsync(int id);
}