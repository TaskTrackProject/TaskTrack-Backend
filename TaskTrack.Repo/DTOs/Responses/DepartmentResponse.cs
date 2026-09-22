
namespace TaskTrack.Repo.DTOs.Responses
{
    public class DepartmentResponse
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public string? DepartmentDescription { get; set; }

        public bool IsActive { get; set; }
    }
}
