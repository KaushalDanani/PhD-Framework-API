using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IFacultyRepository
    {
        Task<List<FacultyResponseDto>> GetAllFacultyAsync();
    }
}
