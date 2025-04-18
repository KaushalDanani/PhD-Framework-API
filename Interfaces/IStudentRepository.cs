using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IStudentRepository
    {
        Task AddStudentAsync(Student student);
        Task<Student> GetStudentWithFacultyAndDepartmentAsync(string email);
        Task<Student> GetStudentByEmailAsync(string email);
        Task<Student?> GetStudentByRegistrationIdAsync(string id);
        Task<Student?> GetStudentByUserIdAsync(string userId);
    }
}
