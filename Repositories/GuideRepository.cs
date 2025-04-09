using Backend.Data;
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
    }
}
