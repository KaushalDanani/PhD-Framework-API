using System.IdentityModel.Tokens.Jwt;
using Backend.CustomExceptions;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;
using Google.Apis.Drive.v3.Data;
using static System.Net.Mime.MediaTypeNames;

namespace Backend.Services
{
    public class ProgressReportService : IProgressReportService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProgressReportRepository _progressReportRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IConfiguration _configuration;
        private readonly IApplicationFileRepository _fileRepository;
        private readonly IPhDTitleRepository _phDTitleRepository;
        private readonly IEmailService _emailService;

        public ProgressReportService(IHttpContextAccessor httpContextAccessor,
            IProgressReportRepository progressReportRepository, IStudentRepository studentRepository,
            IGoogleDriveService googleDriveService, IConfiguration configuration,
            IApplicationFileRepository fileRepository, IPhDTitleRepository titleRepository, IEmailService emailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _progressReportRepository = progressReportRepository;
            _studentRepository = studentRepository;
            _googleDriveService = googleDriveService;
            _configuration = configuration;
            _fileRepository = fileRepository;
            _phDTitleRepository = titleRepository;
            _emailService = emailService;
        }

        public async Task<ServiceResponseDto> UploadNewProgressReportAsync(IFormFile reportFile)
        {
            // Extract user info from JWT
            var token = _httpContextAccessor.HttpContext!.Request.Cookies["AuthToken"];
            //Console.WriteLine("Token:" + token);
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Invalid or expire token");

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userId = jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            //Console.WriteLine($"User Id: {userId}");

            if (string.IsNullOrEmpty(userId))
                throw new UserNotFoundException("Unauthorized User");

            // Get Student from userId which is basically get from jwt bearer
            var associatedStudent = await _studentRepository.GetStudentByUserIdAsync(userId);
            //Console.WriteLine($"## Student: {associatedStudent}");

            if (associatedStudent == null)
            {
                return new ServiceResponseDto
                {
                    IsSuccess = false,
                    Message = "Student is not found"
                };
            }

            var lastReport =
                await _progressReportRepository.GetLastUploadedProgressReportAsync(associatedStudent.RegistrationId);
            var nextReportNo = lastReport != null ? lastReport.ProgressReportNo + 1 : 1;

            // Store report file in Google Drive
            var reportFolderId = _configuration["GoogleDrive:ProgressReportFolderId"];
            string fileName;
            if (associatedStudent == null)
                fileName = reportFile.FileName;
            else
                fileName = "PR-" + associatedStudent.Email?.Split("@")[0] + " - " + nextReportNo;

            try
            {
                using (var fileStream = reportFile.OpenReadStream())
                {
                    var fileData = await _googleDriveService.UploadFileToDriveAsync(fileStream, fileName,
                        reportFile.ContentType, reportFolderId!);

                    // Create an ApplicationFile entity to store in the database
                    var applicationFile = new ApplicationFile
                    {
                        FileId = Guid.NewGuid(),
                        StoredCloudFileId = fileData.ResponseBody.Id,
                        RelatedTable = "ProgressReport",
                        FileName = fileName,
                        MimeType = reportFile.ContentType,
                        FileSize = fileData.ResponseBody.Size ?? 0, // If file size is null, default to 0
                        FilePath =
                            $"https://drive.google.com/uc?export=view&id={fileData.ResponseBody.Id}", // Google Drive link
                        UploadedAt = DateTime.Now
                    };
                    await _fileRepository.AddAsync(applicationFile);

                    // Create a new ProgressReport entry
                    var progressReport = new ProgressReport
                    {
                        RegistrationId = associatedStudent.RegistrationId,
                        ProgressReportNo = nextReportNo,
                        FileId = applicationFile.FileId,
                        SubmissionDate = DateTime.Now,
                        GuideStatus = false, // Default status
                        DeanStatus = false,
                        AcademicSectionStatus = false,
                        ReportStatus = true, // Active by default
                        LastUpdatedAt = DateTime.Now
                    };
                    await _progressReportRepository.AddAsync(progressReport);

                    var progressReportDto = new ProgressReportResponseDto
                    {
                        ProgressReportNo = nextReportNo,
                        FileName = applicationFile.FileName,
                        FilePath = applicationFile.FilePath,
                        FileSize = applicationFile.FileSize,
                        ReportStatus = progressReport.ReportStatus,
                        SubmissionDate = progressReport.SubmissionDate.ToString("F"),
                        GuideStatus = progressReport.GuideStatus,
                        GuideReviewDate = progressReport.GuideReviewDate?.ToString("F"),
                        GuideRemark = progressReport.GuideRemark,
                        DeanStatus = progressReport.DeanStatus,
                        DeanReviewDate = progressReport.DeanReviewDate?.ToString("F"),
                        DeanRemark = progressReport.DeanRemark,
                        AcademicSectionStatus = progressReport.AcademicSectionStatus,
                        AcademicSectionApproveDate = progressReport.AcademicSectionApproveDate?.ToString("F"),
                        AcademicSectionRemark = progressReport.AcademicSectionRemark,
                        LastUpdatedAt = progressReport.LastUpdatedAt.ToString("F")
                    };

                    return new ServiceResponseDto
                    {
                        IsSuccess = true,
                        Message = "Report uploaded successfully",
                        Data = progressReportDto
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponseDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ProgressReportResultDto> GetLatestProgressReportAsync()
        {
            var token = _httpContextAccessor.HttpContext!.Request.Cookies["AuthToken"];
            //Console.WriteLine("Token:" + token);
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Invalid or expire token");

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userId = jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            //Console.WriteLine($"User Id: {userId}");

            if (string.IsNullOrEmpty(userId))
                throw new UserNotFoundException("Unauthorized User");

            // Get Student from userId which is basically get from jwt bearer
            var associatedStudent = await _studentRepository.GetStudentByUserIdAsync(userId);
            //Console.WriteLine($"## Student: {associatedStudent}");

            if (associatedStudent == null)
                throw new InvalidOperationException("Student is not found");

            var lastReport =
                await _progressReportRepository.GetLastUploadedProgressReportAsync(associatedStudent.RegistrationId);
            //Console.WriteLine($"## Latest Progress Report: {lastReport}");

            if (lastReport != null)
            {
                var fileMetaData = await _fileRepository.GetByFileIdAsync(lastReport.FileId);

                var progressReportDto = new ProgressReportResponseDto
                {
                    ProgressReportNo = lastReport.ProgressReportNo,
                    FileName = fileMetaData.FileName,
                    FilePath = fileMetaData.FilePath,
                    FileSize = fileMetaData.FileSize,
                    ReportStatus = lastReport.ReportStatus,
                    SubmissionDate = lastReport.SubmissionDate.ToString("F"),
                    GuideStatus = lastReport.GuideStatus,
                    GuideReviewDate = lastReport.GuideReviewDate?.ToString("F"),
                    GuideRemark = lastReport.GuideRemark,
                    DeanStatus = lastReport.DeanStatus,
                    DeanReviewDate = lastReport.DeanReviewDate?.ToString("F"),
                    DeanRemark = lastReport.DeanRemark,
                    AcademicSectionStatus = lastReport.AcademicSectionStatus,
                    AcademicSectionApproveDate = lastReport.AcademicSectionApproveDate?.ToString("F"),
                    AcademicSectionRemark = lastReport.AcademicSectionRemark,
                    LastUpdatedAt = lastReport.LastUpdatedAt.ToString("F")
                };
                return new ProgressReportResultDto
                {
                    ReportDto = progressReportDto
                };
            }

            return new ProgressReportResultDto
            {
                ReportEntity = lastReport
            };
        }

        public async Task<ProgressReportResponseDto> ChangeLatestReportFileAsync(IFormFile newReportFile)
        {
            var token = _httpContextAccessor.HttpContext!.Request.Cookies["AuthToken"];
            //Console.WriteLine("Token:" + token);
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Invalid or expire token");

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userId = jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            Console.WriteLine($"User Id: {userId}");

            if (string.IsNullOrEmpty(userId))
                throw new UserNotFoundException("Unauthorized User");

            // Get Student from userId which is basically get from jwt bearer
            var associatedStudent = await _studentRepository.GetStudentByUserIdAsync(userId);
            Console.WriteLine($"## Student: {associatedStudent}");

            if (associatedStudent == null)
                throw new InvalidOperationException("Student is not found");

            var lastReport =
                await _progressReportRepository.GetLastUploadedProgressReportAsync(associatedStudent.RegistrationId);
            Console.WriteLine($"## Latest Progress Report: {lastReport}");

            var reportWithFile = await _progressReportRepository.GetProgressReportWithReportFile(lastReport);

            // Delete previous uploaded report file from Drive
            var isFileDeletedFromDrive =
                await _googleDriveService.DeleteFileFromDriveAsync(reportWithFile.ApplicationFile.StoredCloudFileId);
            if (!isFileDeletedFromDrive)
                throw new InvalidOperationException("Report file isn't deleted");


            // Replace old report data to new report data
            try
            {
                var reportFolderId = _configuration["GoogleDrive:ProgressReportFolderId"];
                using (var fileStream = newReportFile.OpenReadStream())
                {
                    var fileData =
                        await _googleDriveService.UploadFileToDriveAsync(fileStream,
                            reportWithFile.ApplicationFile.FileName, newReportFile.ContentType, reportFolderId!);

                    var existingApplicationFile = await _fileRepository.GetByFileIdAsync(lastReport.FileId);
                    if (existingApplicationFile != null)
                    {
                        // Update the fields of the existing file record
                        existingApplicationFile.StoredCloudFileId = fileData.ResponseBody.Id;
                        existingApplicationFile.MimeType = newReportFile.ContentType;
                        existingApplicationFile.FileSize = fileData.ResponseBody.Size ?? 0;
                        existingApplicationFile.FilePath =
                            $"https://drive.google.com/uc?export=view&id={fileData.ResponseBody.Id}"; // Google Drive link
                        existingApplicationFile.UploadedAt = DateTime.Now;

                        // Save the updated application file record to the database
                        await _fileRepository.UpdateAsync(existingApplicationFile);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Report file isn't updated");
            }

            var phdTitleRecord = await _phDTitleRepository.GetPhDTitleAsync(associatedStudent.RegistrationId);
            var guideEmail = phdTitleRecord.Guide.Email;
            var guideName = $"{phdTitleRecord.Guide.FirstName} {phdTitleRecord.Guide.LastName}";

            try
            {
                await _emailService.SendProgressReportUpdatedByStudentEmailToGuideAsync(
                    guideEmail,
                    guideName,
                    $"{associatedStudent.FirstName} {associatedStudent.LastName}", associatedStudent.RegistrationId,
                    associatedStudent.Email,
                    phdTitleRecord.PhDTitleName,
                    phdTitleRecord.ResearchArea,
                    reportWithFile.ApplicationFile.FilePath,
                    reportWithFile.ApplicationFile.FileName);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Guide isn't notify through email, but progress report updated successfully");
            }

            return new ProgressReportResponseDto
            {
                ProgressReportNo = lastReport.ProgressReportNo,
                FileName = reportWithFile.ApplicationFile.FileName,
                FilePath = reportWithFile.ApplicationFile.FilePath,
                FileSize = reportWithFile.ApplicationFile.FileSize,
                ReportStatus = lastReport.ReportStatus,
                SubmissionDate = lastReport.SubmissionDate.ToString("F"),
                GuideStatus = lastReport.GuideStatus,
                GuideReviewDate = lastReport.GuideReviewDate?.ToString("F"),
                GuideRemark = lastReport.GuideRemark,
                DeanStatus = lastReport.DeanStatus,
                DeanReviewDate = lastReport.DeanReviewDate?.ToString("F"),
                DeanRemark = lastReport.DeanRemark,
                AcademicSectionStatus = lastReport.AcademicSectionStatus,
                AcademicSectionApproveDate = lastReport.AcademicSectionApproveDate?.ToString("F"),
                AcademicSectionRemark = lastReport.AcademicSectionRemark,
                LastUpdatedAt = lastReport.LastUpdatedAt.ToString("F")
            };
        }
    }
}
