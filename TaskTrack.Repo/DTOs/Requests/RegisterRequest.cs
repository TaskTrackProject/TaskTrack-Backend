using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Repo.DTOs.Requests;

public class RegisterRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [StringLength(72)]
    public string Password { get; set; } = string.Empty;
}