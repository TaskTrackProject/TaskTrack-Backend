using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Repo.DTOs.Responses;

namespace TaskTrack.Service.Interfaces;

public interface ITagService
{
    Task<List<TagResponse>> GetAllAsync();

    Task<TagResponse?> GetByIdAsync(int id);

    Task<TagResponse> CreateAsync(TagRequest request);

    Task<TagResponse?> UpdateAsync(
        int id,
        TagRequest request);

    Task<bool> DeleteAsync(int id);
}