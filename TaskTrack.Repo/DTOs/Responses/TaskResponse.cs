namespace TaskTrack.Repo.DTOs.Responses
{
    public class TaskResponse
    {
        public int TaskId { get; set; }

        public string TaskName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public short Status { get; set; }

        public short Priority { get; set; }

        public DateOnly? DueDate { get; set; }

        public int ProjectId { get; set; }  

        public bool IsActive { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public List<TagResponse> Tags { get; set; } = new();
    }
}
