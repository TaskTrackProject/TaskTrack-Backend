
namespace TaskTrack.Repo.DTOs.Responses
{
    public class ProjectResponse
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public int Status { get; set; }

        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
