using Backend.Entities;
namespace Backend.DTOs
{
    public class ProgressReportResultDto
    {
        public ProgressReport? ReportEntity { get; set; }
        public ProgressReportResponseDto? ReportDto { get; set; }
    }
}