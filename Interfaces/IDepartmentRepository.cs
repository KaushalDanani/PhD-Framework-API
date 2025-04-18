using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<List<DepartmentResponseDto>> GetDepartmentListByFacultyAsync(int facultyId);
    }
}
