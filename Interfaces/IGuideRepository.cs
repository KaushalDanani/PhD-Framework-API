using Backend.DTOs;
using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IGuideRepository
    {
        Task<Guide> GetGuideByEmailAsync(string email);
        Task<Guide> GetGuideByUserIdAsync(string userId);
        Task<List<GetGuideListResponseDto>> GetAllGuideAsListAsync();
        Task<Guide> GetGuideWithFacultyAndDepartmentAsync(string email);
    }
}
