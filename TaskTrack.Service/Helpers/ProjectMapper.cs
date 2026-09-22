using TaskTrack.Repo.DTOs.Responses;
using TaskTrack.Repo.Models;

namespace TaskTrack.Service.Helpers;

public static class ProjectMapper
{
    public static ProjectResponse ToResponse(Project project)
    {
        return new ProjectResponse
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Status = project.Status,
            DepartmentId = project.DepartmentId,
            DepartmentName = project.Department?.DepartmentName,
            IsActive = project.IsActive,
            CreatedDate = project.CreatedDate
        };
    }

    public static ProjectDetailResponse ToDetailResponse(
        Project project)
    {
        return new ProjectDetailResponse
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Status = project.Status,
            DepartmentId = project.DepartmentId,
            DepartmentName = project.Department?.DepartmentName,
            IsActive = project.IsActive,
            CreatedDate = project.CreatedDate,

            Tasks = project.Tasks
                .Select(task => new TaskResponse
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
                })
                .ToList()
        };
    }
}