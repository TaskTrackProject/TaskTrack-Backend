using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Repo.DTOs.Responses;

namespace TaskTrack.Service.Interfaces;

public interface IAccountService
{
    Task<List<AccountResponse>> GetAllAsync();

    Task<AccountResponse?> GetByIdAsync(int id);

    Task<AccountResponse?> UpdateAsync(int id, UpdateAccountRequest request);

    Task<bool> DeleteAsync(int id);
}