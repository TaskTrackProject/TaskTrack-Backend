using TaskTrack.Repo.DTOs.Requests;

namespace TaskTrack.Service.Helpers;

public class TagValidator
{
    public void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "Tag ID must be greater than 0.");
        }
    }

    public void ValidateRequest(TagRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            throw new ArgumentException(
                "Tag name is required.");
        }

        if (request.TagName.Trim().Length > 100)
        {
            throw new ArgumentException(
                "Tag name cannot exceed 100 characters.");
        }

        if (request.Color?.Length > 20)
        {
            throw new ArgumentException(
                "Tag color cannot exceed 20 characters.");
        }
    }
}