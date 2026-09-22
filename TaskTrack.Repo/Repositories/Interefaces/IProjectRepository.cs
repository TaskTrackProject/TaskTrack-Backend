using TaskTrack.Repo.Models;
using Tasks = System.Threading.Tasks;

namespace TaskTrack.Repo.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllActiveAsync();

    Task<Project?> GetByIdAsync(int id);

    Task<List<Project>> GetByDepartmentAsync(int departmentId);

    Task<List<Project>> SearchAsync(
        string? name,
        int? status,
        int? departmentId);

    Tasks.Task AddAsync(Project project);

    Tasks.Task UpdateAsync(Project project);

    Tasks.Task DeleteAsync(Project project);

    Task<bool> HasTasksAsync(int projectId);

    Task<bool> DepartmentExistsAsync(int departmentId);
}