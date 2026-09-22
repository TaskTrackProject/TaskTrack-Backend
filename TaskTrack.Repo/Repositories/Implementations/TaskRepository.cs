using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using Tasks = System.Threading.Tasks;

namespace TaskTrack.Repo.Repositories.Implementations;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementDbContext _context;

    public TaskRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<Models.Task>> GetAllActiveAsync()
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Tags)
            .Where(t => t.IsActive)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Models.Task?> GetByIdAsync(int id)
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.TaskId == id);
    }

    public async Task<List<Models.Task>> GetByProjectAsync(
        int projectId)
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Tags)
            .Where(t =>
                t.IsActive &&
                t.ProjectId == projectId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Models.Task>> SearchAsync(
        string? title,
        int? status,
        int? priority,
        int? projectId)
    {
        var query = _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Tags)
            .Where(t => t.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(t =>
                t.Title.Contains(title));
        }

        if (status.HasValue)
        {
            query = query.Where(t =>
                t.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t =>
                t.Priority == priority.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(t =>
                t.ProjectId == projectId.Value);
        }

        return await query
            .AsNoTracking()
            .ToListAsync();
    }

    public async Tasks.Task AddAsync(Models.Task task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    public async Tasks.Task UpdateAsync(Models.Task task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Tasks.Task SetTagsAsync(Models.Task task, List<int> tagIds)
    {
        var tags = await _context.Tags
            .Where(tag => tagIds.Contains(tag.TagId))
            .ToListAsync();

        task.Tags.Clear();
        foreach (var tag in tags)
        {
            task.Tags.Add(tag);
        }

        await _context.SaveChangesAsync();
    }

    public async Tasks.Task DeleteAsync(Models.Task task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ProjectExistsAsync(
        int projectId)
    {
        return await _context.Projects
            .AnyAsync(p =>
                p.ProjectId == projectId &&
                p.IsActive);
    }
}