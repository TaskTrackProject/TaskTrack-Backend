namespace TaskTrack.Repo.DTOs.Responses;

public class AccountResponse
{
    public int AccountId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public short Role { get; set; }

    public DateTime CreatedDate { get; set; }
}