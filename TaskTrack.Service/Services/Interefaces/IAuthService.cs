using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Repo.DTOs.Responses;

namespace TaskTrack.Service.Interfaces;

public interface IAuthService
{
    Task<AccountResponse> RegisterAsync(RegisterRequest request);

    Task<AuthenticationResponse?> LoginAsync(LoginRequest request);
}