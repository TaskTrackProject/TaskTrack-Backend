using TaskTrack.Repo.DTOs.Responses;
using TaskTrack.Repo.Models;

namespace TaskTrack.Service.Helpers;

public static class TagMapper
{
    public static TagResponse ToResponse(Tag tag)
    {
        return new TagResponse
        {
            TagId = tag.TagId,
            TagName = tag.TagName,
            Color = tag.Color
        };
    }
}