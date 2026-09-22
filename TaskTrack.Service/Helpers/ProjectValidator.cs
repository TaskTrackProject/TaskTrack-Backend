using TaskTrack.Repo.DTOs.Requests;

namespace TaskTrack.Service.Helpers
{
    public class ProjectValidator
    {
        public void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Project ID must be greater than 0.");
            }
        }

        public void ValidateStatus(int? status)
        {
            if (status.HasValue &&
                (status.Value < 0 || status.Value > 3))
            {
                throw new ArgumentException(
                    "Project status must be between 0 and 3.");
            }
        }

        public void ValidateRequest(ProjectRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.ProjectName))
            {
                throw new ArgumentException(
                    "Project name is required.");
            }

            if (request.ProjectName.Trim().Length > 200)
            {
                throw new ArgumentException(
                    "Project name cannot exceed 200 characters.");
            }

            if (request.Description?.Length > 1000)
            {
                throw new ArgumentException(
                    "Project description cannot exceed 1000 characters.");
            }

            ValidateStatus(request.Status);

            if (request.DepartmentId <= 0)
            {
                throw new ArgumentException(
                    "Department ID must be greater than 0.");
            }

            if (request.EndDate.HasValue &&
                request.StartDate > request.EndDate.Value)
            {
                throw new ArgumentException(
                    "End date cannot be earlier than start date.");
            }
        }
    }
}
