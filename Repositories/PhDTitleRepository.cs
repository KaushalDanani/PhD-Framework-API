using Backend.Data;
using Backend.Entities;
using Backend.Interfaces;

namespace Backend.Repositories
{
    public class PhDTitleRepository : IPhDTitleRepository
    {  
        private readonly ApplicationDbContext _context;

        public PhDTitleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddPhdTitle(PhDTitle phDTitle)
        {
            await _context.PhDTitles.AddAsync(phDTitle);
            await _context.SaveChangesAsync();
        }
    }
}
