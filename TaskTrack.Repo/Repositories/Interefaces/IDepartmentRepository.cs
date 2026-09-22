using TaskTrack.Repo.Models;
using Tasks = System.Threading.Tasks;

namespace TaskTrack.Repo.Repositories.Interfaces;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAllActiveAsync();

    Task<Department?> GetByIdAsync(int id);

    Task<List<Department>> SearchByNameAsync(string name);

    Tasks.Task AddAsync(Department department);

    Tasks.Task UpdateAsync(Department department);

    Tasks.Task DeleteAsync(Department department);
    Task<bool> HasProjectsAsync(int departmentId);
}