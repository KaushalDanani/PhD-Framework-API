using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IStudentRepository
    {
        Task CreateStudentAsync(Student student);
        //Task UpdateStudentAsync(Student student);
        Task<Student> GetStudentByIdAsync(string registrationNo);

    }
}
