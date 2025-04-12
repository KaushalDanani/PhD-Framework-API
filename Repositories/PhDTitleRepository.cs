using Backend.Data;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> GetPhDTitleRecordAsync(string phdId)
        {
            var record = await _context.PhDTitles.FirstOrDefaultAsync(phDt => phDt.RegistrationId == phdId);

            if(record != null)
                return true;

            return false;
        }
    }
}
