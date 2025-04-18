using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class GuideRepository : IGuideRepository
    {
        private readonly ApplicationDbContext _context;

        public GuideRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guide> GetGuideByEmailAsync(string email)
        {
            return (await _context.Guides.FirstOrDefaultAsync(g => g.Email == email))!;
        }

        public async Task<Guide> GetGuideByUserIdAsync(string userId)
        {
            return (await _context.Guides.FirstOrDefaultAsync(g => g.UserId == userId))!;
        }

        public async Task<List<GetGuideListResponseDto>> GetAllGuideAsListAsync()
        {
            return await _context.Guides
                .Select(g => new GetGuideListResponseDto
                {
                    GuideId = g.GuideId,
                    FullName = g.FirstName + " " + g.FatherName + " " + g.LastName
                })
                .ToListAsync();
        }

        public async Task<Guide> GetGuideWithFacultyAndDepartmentAsync(string email)
        {
            return (await _context.Guides
                .Include(nav => nav.Faculty)
                .Include(nav => nav.Department)
                .FirstOrDefaultAsync(s => s.Email == email))!;
        }
    }
}
