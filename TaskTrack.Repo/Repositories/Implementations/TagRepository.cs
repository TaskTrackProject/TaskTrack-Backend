using Microsoft.EntityFrameworkCore;
using Tasks = System.Threading.Tasks;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;

namespace TaskTrack.Repo.Repositories.Implementations;

public class TagRepository : ITagRepository
{
    private readonly TaskManagementDbContext _context;

    public TagRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tag>> GetAllAsync()
    {
        return await _context.Tags
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(t => t.TagId == id);
    }

    public async Task<bool> ExistsByNameAsync(
        string tagName,
        int? excludeId = null)
    {
        var query = _context.Tags
            .Where(t => t.TagName == tagName);

        if (excludeId.HasValue)
        {
            query = query.Where(t =>
                t.TagId != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> HasTasksAsync(int tagId)
    {
        return await _context.Tasks
            .AnyAsync(t =>
                t.Tags.Any(tag => tag.TagId == tagId));
    }

    public async Tasks.Task AddAsync(Tag tag)
    {
        await _context.Tags.AddAsync(tag);
        await _context.SaveChangesAsync();
    }

    public async Tasks.Task UpdateAsync(Tag tag)
    {
        _context.Tags.Update(tag);
        await _context.SaveChangesAsync();
    }

    public async Tasks.Task DeleteAsync(Tag tag)
    {
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
    }
}