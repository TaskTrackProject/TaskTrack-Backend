using TaskTrack.Repo.Models;
using Tasks = System.Threading.Tasks;

namespace TaskTrack.Repo.Repositories.Interfaces;

public interface ITagRepository
{
    Task<List<Tag>> GetAllAsync();

    Task<Tag?> GetByIdAsync(int id);

    Task<bool> ExistsByNameAsync(
        string tagName,
        int? excludeId = null);

    Task<bool> HasTasksAsync(int tagId);

    Tasks.Task AddAsync(Tag tag);

    Tasks.Task UpdateAsync(Tag tag);

    Tasks.Task DeleteAsync(Tag tag);
}