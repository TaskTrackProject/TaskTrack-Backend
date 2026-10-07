using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Repo.DTOs.Requests;

public class UpdateAccountRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string FullName { get; set; } = string.Empty;

    [Range(0, 1)]
    public short Role { get; set; }
}