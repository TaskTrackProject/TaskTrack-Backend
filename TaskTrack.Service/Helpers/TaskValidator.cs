using TaskTrack.Repo.DTOs.Requests;

namespace TaskTrack.Service.Helpers;

public class TaskValidator
{
    public void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "Task ID must be greater than 0.");
        }
    }

    public void ValidateStatus(int? status)
    {
        if (status.HasValue &&
            (status.Value < 0 || status.Value > 3))
        {
            throw new ArgumentException(
                "Task status must be between 0 and 3.");
        }
    }

    public void ValidatePriority(int? priority)
    {
        if (priority.HasValue &&
            (priority.Value < 0 || priority.Value > 3))
        {
            throw new ArgumentException(
                "Task priority must be between 0 and 3.");
        }
    }

    public void ValidateRequest(TaskRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException(
                "Task title is required.");
        }

        if (request.Title.Trim().Length > 200)
        {
            throw new ArgumentException(
                "Task title cannot exceed 200 characters.");
        }

        if (request.Description?.Length > 1000)
        {
            throw new ArgumentException(
                "Task description cannot exceed 1000 characters.");
        }

        ValidateStatus(request.Status);
        ValidatePriority(request.Priority);

        if (request.ProjectId <= 0)
        {
            throw new ArgumentException(
                "Project ID must be greater than 0.");
        }
    }
}