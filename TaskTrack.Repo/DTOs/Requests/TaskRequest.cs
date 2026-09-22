using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Repo.DTOs.Requests
{
    public class TaskRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public int Status { get; set; }

        public int Priority { get; set; }

        public DateOnly? DueDate { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public List<int> TagIds { get; set; } = new();
    }
}
