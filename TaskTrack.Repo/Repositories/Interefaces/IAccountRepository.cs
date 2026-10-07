using TaskTrack.Repo.Models;
using Tasks = System.Threading.Tasks;

namespace TaskTrack.Repo.Repositories.Interfaces;

public interface IAccountRepository
{
    Tasks.Task<SystemAccount?> GetByEmailAsync(string email);

    Tasks.Task<List<SystemAccount>> GetAllAsync();

    Tasks.Task<SystemAccount?> GetByIdAsync(int id);

    Tasks.Task AddAsync(SystemAccount account);

    Tasks.Task UpdateAsync(SystemAccount account);

    Tasks.Task DeleteAsync(SystemAccount account);

    Tasks.Task<bool> HasCreatedTasksAsync(int accountId);
}