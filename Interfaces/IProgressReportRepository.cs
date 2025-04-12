using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IProgressReportRepository
    {
        Task AddAsync(ProgressReport report);
        Task<ProgressReport?> GetLastUploadedProgressReportAsync(string phdId);
    }
}
