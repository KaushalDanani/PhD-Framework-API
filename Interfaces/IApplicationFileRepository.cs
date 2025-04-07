using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IApplicationFileRepository
    {
        Task<ApplicationFile> AddAsync(ApplicationFile file);
        Task<ApplicationFile?> GetByFileIdAsync(Guid? fileId);
        Task UpdateAsync(ApplicationFile file);
    }
}
