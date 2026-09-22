using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Repo.DTOs.Responses;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.Helpers;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Implementations;

public class TagService : ITagService
{
    private readonly ITagRepository _repository;
    private readonly TagValidator _validator;

    public TagService(
        ITagRepository repository,
        TagValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<List<TagResponse>> GetAllAsync()
    {
        var tags = await _repository.GetAllAsync();

        return tags
            .Select(TagMapper.ToResponse)
            .ToList();
    }

    public async Task<TagResponse?> GetByIdAsync(int id)
    {
        _validator.ValidateId(id);

        var tag = await _repository.GetByIdAsync(id);

        if (tag == null)
            return null;

        return TagMapper.ToResponse(tag);
    }

    public async Task<TagResponse> CreateAsync(
        TagRequest request)
    {
        _validator.ValidateRequest(request);

        var tagName = request.TagName.Trim();

        var exists =
            await _repository.ExistsByNameAsync(tagName);

        if (exists)
        {
            throw new ArgumentException(
                "Tag name already exists.");
        }

        var tag = new Tag
        {
            TagName = tagName,

            Color = string.IsNullOrWhiteSpace(request.Color)
                ? null
                : request.Color.Trim()
        };

        await _repository.AddAsync(tag);

        return TagMapper.ToResponse(tag);
    }

    public async Task<TagResponse?> UpdateAsync(
        int id,
        TagRequest request)
    {
        _validator.ValidateId(id);
        _validator.ValidateRequest(request);

        var tag = await _repository.GetByIdAsync(id);

        if (tag == null)
            return null;

        var tagName = request.TagName.Trim();

        var exists =
            await _repository.ExistsByNameAsync(
                tagName,
                id);

        if (exists)
        {
            throw new ArgumentException(
                "Tag name already exists.");
        }

        tag.TagName = tagName;

        tag.Color = string.IsNullOrWhiteSpace(request.Color)
            ? null
            : request.Color.Trim();

        await _repository.UpdateAsync(tag);

        return TagMapper.ToResponse(tag);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _validator.ValidateId(id);

        var tag = await _repository.GetByIdAsync(id);

        if (tag == null)
            return false;

        var hasTasks =
            await _repository.HasTasksAsync(id);

        if (hasTasks)
        {
            throw new InvalidOperationException(
                "Cannot delete tag because it is used by tasks.");
        }

        await _repository.DeleteAsync(tag);

        return true;
    }
}