using Backend.DTOs;
using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IProgressReportService
    {
        Task<ServiceResponseDto> UploadNewProgressReportAsync(IFormFile reportFile);
        Task<ProgressReportResultDto> GetLatestProgressReportAsync();
    }
}
