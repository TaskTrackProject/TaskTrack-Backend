using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Repo.DTOs.Responses;
using TaskTrack.Repo.Models;

namespace TaskTrack.Service.Interfaces;

public interface IDepartmentService
{
    Task<List<DepartmentResponse>> GetAllAsync();

    Task<DepartmentDetailResponse?> GetByIdAsync(int id);

    Task<List<DepartmentResponse>> SearchAsync(string name);

    Task<DepartmentResponse> CreateAsync(DepartmentRequest request);

    Task<DepartmentResponse?> UpdateAsync(
        int id,
        DepartmentRequest request);

    Task<bool> DeleteAsync(int id);
}