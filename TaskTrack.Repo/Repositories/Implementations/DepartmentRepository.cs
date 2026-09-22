using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using Tasks = System.Threading.Tasks;

namespace TaskTrack.Repo.Repositories.Implementations;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly TaskManagementDbContext _context;

    public DepartmentRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<Department>> GetAllActiveAsync()
    {
        return await _context.Departments
            .Where(d => d.IsActive)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Department?> GetByIdAsync(int id)
    {
        return await _context.Departments
            .Include(d => d.Projects)
            .FirstOrDefaultAsync(d => d.DepartmentId == id);
    }

    public async Task<List<Department>> SearchByNameAsync(string name)
    {
        return await _context.Departments
            .Where(d => d.IsActive &&
                        d.DepartmentName.Contains(name))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Tasks.Task AddAsync(Department department)
    {
        await _context.Departments.AddAsync(department);
        await _context.SaveChangesAsync();
    }

    public async Tasks.Task UpdateAsync(Department department)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
    }

    public async Tasks.Task DeleteAsync(Department department)
    {
        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasProjectsAsync(int departmentId)
    {
        return await _context.Projects
            .AnyAsync(p => p.DepartmentId == departmentId);
    }
}