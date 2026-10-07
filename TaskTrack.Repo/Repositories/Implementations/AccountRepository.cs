using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using Tasks = System.Threading.Tasks;

namespace TaskTrack.Repo.Repositories.Implementations;

public class AccountRepository : IAccountRepository
{
    private readonly TaskManagementDbContext _context;

    public AccountRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public Tasks.Task<SystemAccount?> GetByEmailAsync(string email) =>
        _context.SystemAccounts.FirstOrDefaultAsync(account => account.Email == email);

    public Tasks.Task<List<SystemAccount>> GetAllAsync() =>
        _context.SystemAccounts.AsNoTracking().OrderBy(account => account.AccountId).ToListAsync();

    public Tasks.Task<SystemAccount?> GetByIdAsync(int id) =>
        _context.SystemAccounts.FirstOrDefaultAsync(account => account.AccountId == id);

    public async Tasks.Task AddAsync(SystemAccount account)
    {
        await _context.SystemAccounts.AddAsync(account);
        await _context.SaveChangesAsync();
    }

    public async Tasks.Task UpdateAsync(SystemAccount account)
    {
        await _context.SaveChangesAsync();
    }

    public async Tasks.Task DeleteAsync(SystemAccount account)
    {
        _context.SystemAccounts.Remove(account);
        await _context.SaveChangesAsync();
    }

    public Tasks.Task<bool> HasCreatedTasksAsync(int accountId) =>
        _context.Tasks.AnyAsync(task => task.CreatedByAccountId == accountId);
}