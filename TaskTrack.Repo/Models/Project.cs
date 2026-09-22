using System;
using System.Collections.Generic;

namespace TaskTrack.Repo.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public short Status { get; set; } // Status: 0 = Not Started, 1 = In Progress, 2 = Completed, 3 = On Hold

    public int DepartmentId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
