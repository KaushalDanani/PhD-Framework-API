using Backend.Data;
using Backend.DTOs;
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

        public async Task<ProgressReport> GetProgressReportWithReportFile(ProgressReport progressReport)
        {
            return (await _context.ProgressReports
                .Include(nav => nav.ApplicationFile)
                .FirstOrDefaultAsync(pr =>
                    pr.RegistrationId == progressReport.RegistrationId &&
                    pr.ProgressReportNo == progressReport.ProgressReportNo))!;
        }

        public async Task<List<ProgressReportRequestsMetaDataDto>> GetStudentsPendingReportsByGuideIdAsync(int guideId)
        {
            var result = await (
                from report in _context.ProgressReports.AsNoTracking()
                join title in _context.PhDTitles.AsNoTracking() on report.RegistrationId equals title.RegistrationId
                join student in _context.Students.AsNoTracking() on report.RegistrationId equals student.RegistrationId
                join user in _context.Users.AsNoTracking() on student.UserId equals user.Id
                join reportFile in _context.ApplicationFiles.AsNoTracking() on report.FileId equals reportFile.FileId
                join profileImage in _context.ApplicationFiles.AsNoTracking() on user.ProfileImageId equals profileImage.FileId into profileJoin
                from profileImage in profileJoin.DefaultIfEmpty() // Handle null image
                where title.GuideId == guideId
                      && title.Status == true
                      && report.GuideStatus == false
                      && report.ReportStatus == true
                select new ProgressReportRequestsMetaDataDto
                {
                    RegistrationId = student.RegistrationId,
                    ReportNo = report.ProgressReportNo,
                    isChecked = false,
                    Title = title.PhDTitleName,
                    ResearchArea = title.ResearchArea,
                    StudentName = student.FirstName + " " + student.LastName,
                    ProfileImageUrl = profileImage != null ? profileImage.FilePath : null,
                    ReportFileName = reportFile.FileName,
                    ReportFileUrl = reportFile.FilePath,
                    SubmittedOn = report.SubmissionDate.ToString("F")
                }).ToListAsync();

            return result;
        }

        public async Task<List<ProgressReport>> GetSelectedProgressReportsByKeyCombinationsAsync(List<ReportKeyDto> keys)
        {
            var registrationIds = keys.Select(k => k.RegistrationId).Distinct().ToList();

            return await _context.ProgressReports
                .Where(pr => registrationIds.Contains(pr.RegistrationId) && pr.ReportStatus)
                .ToListAsync();
        }
    }
}
