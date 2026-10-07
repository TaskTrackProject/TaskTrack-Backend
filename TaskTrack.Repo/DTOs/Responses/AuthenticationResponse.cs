namespace TaskTrack.Repo.DTOs.Responses;

public class AuthenticationResponse
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public AccountResponse Account { get; set; } = new();
}