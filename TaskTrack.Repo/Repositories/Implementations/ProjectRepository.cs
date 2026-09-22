using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using Tasks = System.Threading.Tasks;

namespace TaskTrack.Repo.Repositories.Implementations;

public class ProjectRepository : IProjectRepository
{
    private readonly TaskManagementDbContext _context;

    public ProjectRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<Project>> GetAllActiveAsync()
    {
        return await _context.Projects
            .Include(p => p.Department)
            .Where(p => p.IsActive)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects
            .Include(p => p.Department)
            .Include(p => p.Tasks)
                .ThenInclude(task => task.Tags)
            .FirstOrDefaultAsync(p => p.ProjectId == id);
    }

    public async Task<List<Project>> GetByDepartmentAsync(
        int departmentId)
    {
        return await _context.Projects
            .Include(p => p.Department)
            .Where(p => p.IsActive &&
                        p.DepartmentId == departmentId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Project>> SearchAsync(
        string? name,
        int? status,
        int? departmentId)
    {
        var query = _context.Projects
            .Include(p => p.Department)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p =>
                p.ProjectName.Contains(name));
        }

        if (status.HasValue)
        {
            query = query.Where(p =>
                p.Status == status.Value);
        }

        if (departmentId.HasValue)
        {
            query = query.Where(p =>
                p.DepartmentId == departmentId.Value);
        }

        return await query
            .AsNoTracking()
            .ToListAsync();
    }

    public async Tasks.Task AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
    }

    public async Tasks.Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Tasks.Task DeleteAsync(Project project)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasTasksAsync(int projectId)
    {
        return await _context.Tasks
            .AnyAsync(t => t.ProjectId == projectId);
    }

    public async Task<bool> DepartmentExistsAsync(
        int departmentId)
    {
        return await _context.Departments
            .AnyAsync(d => d.DepartmentId == departmentId &&
                           d.IsActive);
    }
}