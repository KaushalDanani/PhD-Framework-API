using Backend.DTOs;
using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IProgressReportRepository
    {
        Task AddAsync(ProgressReport report);
        Task<ProgressReport?> GetLastUploadedProgressReportAsync(string phdId);
        Task<ProgressReport> GetProgressReportWithReportFile(ProgressReport progressReport);
        Task<List<ProgressReportRequestsMetaDataDto>> GetStudentsPendingReportsByGuideIdAsync(int guideId);
        Task<List<ProgressReport>> GetSelectedProgressReportsByKeyCombinationsAsync(List<ReportKeyDto> keys);
}
}
