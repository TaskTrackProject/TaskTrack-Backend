// Status: 0 = To Do, 1 = In Progress, 2 = Done, 3 = Cancelled
// Priority: 0 = Low, 1 = Medium, 2 = High, 3 = Critical

namespace TaskTrack.Repo.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public short Status { get; set; }

    public short Priority { get; set; }

    public DateOnly? DueDate { get; set; }

    public int ProjectId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
