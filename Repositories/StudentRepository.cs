using Backend.Data;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddStudentAsync(Student student)
        {
            await _context.Students.AddAsync(student);
        }

        public async Task<Student> GetStudentByEmailAsync(string email)
        {
            return (await _context.Students.FirstOrDefaultAsync(s => s.Email == email))!;
        }

        public async Task<Student?> GetStudentByRegistrationIdAsync(string id)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.RegistrationId == id);
        }

        public async Task<Student> GetStudentByUserIdAsync(string userId)
        {
            return (await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId))!;
        }
    }
}
