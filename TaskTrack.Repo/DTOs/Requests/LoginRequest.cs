using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Repo.DTOs.Requests;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}