using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Repo.DTOs.Responses;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.Helpers;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Implementations;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;

    public AccountService(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AccountResponse>> GetAllAsync() =>
        (await _repository.GetAllAsync()).Select(AuthService.ToResponse).ToList();

    public async Task<AccountResponse?> GetByIdAsync(int id)
    {
        ValidateId(id);
        var account = await _repository.GetByIdAsync(id);
        return account is null ? null : AuthService.ToResponse(account);
    }

    public async Task<AccountResponse?> UpdateAsync(int id, UpdateAccountRequest request)
    {
        ValidateId(id);
        var account = await _repository.GetByIdAsync(id);
        if (account is null)
            return null;

        account.FullName = request.FullName.Trim();
        account.Role = request.Role;
        await _repository.UpdateAsync(account);
        return AuthService.ToResponse(account);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        ValidateId(id);
        var account = await _repository.GetByIdAsync(id);
        if (account is null)
            return false;
        if (await _repository.HasCreatedTasksAsync(id))
            throw new InvalidOperationException("Cannot delete this account because it has created tasks.");

        await _repository.DeleteAsync(account);
        return true;
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Account ID must be greater than 0.");
    }
}