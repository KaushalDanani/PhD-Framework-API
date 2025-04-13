using Backend.CustomExceptions;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressReportController : ControllerBase
    {
        private readonly IProgressReportService _progressReportService;

        public ProgressReportController(IProgressReportService progressReportService)
        {
            _progressReportService = progressReportService;
        }

        [Authorize]
        [HttpPost("upload-new")]
        public async Task<IActionResult> UploadProgressReport(IFormFile report)
        {
            if (report.Length == 0)
            {
                return BadRequest(new { message = "File isn't uploaded." });
            }

            if (report.ContentType != "application/pdf")
                return BadRequest(new { message = "Only PDF files are allowed."});

            if (report.Length > 6 * 1024 * 1024)
            {
                return BadRequest(new { message = "File size is too large. Maximum allowed size is 6MB." });
            }

            try
            {
                var result = await _progressReportService.UploadNewProgressReportAsync(report);
                if (result.IsSuccess)
                    return Ok(new { message = result.Message, reportData = result.Data });
            }
            catch (InvalidOperationException ioe)
            {
                return BadRequest(new { message = ioe.Message });
            }
            catch (UserNotFoundException ue)
            {
                return Unauthorized(new { message = ue.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

            return BadRequest(new { message = "Report file isn't uploaded, Try again later!" });
        }

        [Authorize]
        [HttpGet("latest-progress-report")]
        public async Task<IActionResult> GetLastProgressReport()
        {
            try
            {
                var latestReport = await _progressReportService.GetLatestProgressReportAsync();
                if (latestReport.ReportDto != null)
                    return Ok(new { reportData = latestReport.ReportDto });
                if (latestReport.ReportEntity != null)
                    return Ok(new { reportData = latestReport.ReportEntity });

                return Ok();
            }
            catch (InvalidOperationException ioe)
            {
                return BadRequest(new { message = ioe.Message }); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPatch("update-latest-report")]
        public async Task<IActionResult> UpdateLatestProgressReportFile(IFormFile report)
        {
            if (report.Length == 0)
            {
                return BadRequest(new { message = "File isn't uploaded." });
            }

            if (report.ContentType != "application/pdf")
                return BadRequest(new { message = "Only PDF files are allowed." });

            if (report.Length > 6 * 1024 * 1024)
            {
                return BadRequest(new { message = "File size is too large. Maximum allowed size is 6MB." });
            }

            try
            {
                var result = await _progressReportService.ChangeLatestReportFileAsync(report);
                return Ok(new { message = "Report file updated successfully", reportData = result });
            }
            catch (InvalidOperationException ioe)
            {
                return BadRequest(new { message = ioe.Message });
            }
            catch (UserNotFoundException ue)
            {
                return Unauthorized(new { message = ue.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
