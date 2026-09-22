using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Repo.DTOs.Requests
{
    public class ProjectRequest
    {
        [Required]
        [StringLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [Range(0, 3)]
        public int Status { get; set; }

        [Required]
        public int DepartmentId { get; set; }

    }
}
