using Backend.Data;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class ApplicationFileRepository : IApplicationFileRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationFileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationFile> AddAsync(ApplicationFile file)
        {
            _context.ApplicationFiles.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }

        public async Task<ApplicationFile?> GetByFileIdAsync(Guid? fileId)
        {
            return await _context.ApplicationFiles.FirstOrDefaultAsync(f => f.FileId == fileId);
        }

        public async Task UpdateAsync(ApplicationFile file)
        {
            _context.ApplicationFiles.Update(file);
            await _context.SaveChangesAsync();
        }
    }
}
