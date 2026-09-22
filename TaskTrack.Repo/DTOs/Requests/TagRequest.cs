using System.ComponentModel.DataAnnotations;
namespace TaskTrack.Repo.DTOs.Requests
{
    public class TagRequest
    {
        [Required]
        [StringLength(100)]
        public string TagName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Color { get; set; }
    }
}
