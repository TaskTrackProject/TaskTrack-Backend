using Tasks = System.Threading.Tasks;

namespace TaskTrack.Repo.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<List<Models.Task>> GetAllActiveAsync();

    Task<Models.Task?> GetByIdAsync(int id);

    Task<List<Models.Task>> GetByProjectAsync(int projectId);

    Task<List<Models.Task>> SearchAsync(
        string? title,
        int? status,
        int? priority,
        int? projectId);

    Tasks.Task AddAsync(Models.Task task);

    Tasks.Task UpdateAsync(Models.Task task);

    Tasks.Task SetTagsAsync(Models.Task task, List<int> tagIds);

    Tasks.Task DeleteAsync(Models.Task task);

    Task<bool> ProjectExistsAsync(int projectId);
}