using Backend.Data;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class ProgressReportRepository : IProgressReportRepository
    {
        private readonly ApplicationDbContext _context;
        public ProgressReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProgressReport report)
        {
            await _context.ProgressReports.AddAsync(report);
            await _context.SaveChangesAsync();
        }

        public async Task<ProgressReport?> GetLastUploadedProgressReportAsync(string phdId)
        {
            return (await _context.ProgressReports
                .Where(r => r.RegistrationId == phdId)
                .OrderByDescending(pr => pr.ProgressReportNo)
                .FirstOrDefaultAsync())!;
        }
    }
}
