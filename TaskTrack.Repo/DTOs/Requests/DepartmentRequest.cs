using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Repo.DTOs.Requests
{
    public class DepartmentRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string? DepartmentName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? DepartmentDescription { get; set; }
    }
}
