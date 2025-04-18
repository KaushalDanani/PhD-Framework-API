using Backend.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class FacultyRepository : IFacultyRepository
    {
        private readonly ApplicationDbContext _context;

        public FacultyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FacultyResponseDto>> GetAllFacultyAsync()
        {
            return await _context.Faculties
                .Select(f => new FacultyResponseDto { Id = f.Id, Name = f.Name })
                .ToListAsync();
        }
    }
}
