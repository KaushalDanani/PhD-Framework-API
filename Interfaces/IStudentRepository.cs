using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> GetStudentByEmailAsync(string email);
        Task<Student?> GetStudentByRegistrationIdAsync(string id);
    }
}
