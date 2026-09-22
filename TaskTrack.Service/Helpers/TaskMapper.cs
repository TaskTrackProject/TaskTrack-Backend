using TaskTrack.Repo.DTOs.Responses;

namespace TaskTrack.Service.Helpers;

public static class TaskMapper
{
    public static TaskResponse ToResponse(Repo.Models.Task task)
    {
        return new TaskResponse
        {
            TaskId = task.TaskId,
            TaskName = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            IsActive = task.IsActive,
            CreatedDate = task.CreatedDate,
            ModifiedDate = task.ModifiedDate
            ,Tags = task.Tags
                .Select(tag => new TagResponse
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName,
                    Color = tag.Color
                })
                .ToList()
        };
    }
}