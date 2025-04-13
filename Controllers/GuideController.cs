using Backend.CustomExceptions;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuideController : ControllerBase
    {
        private readonly IGuideService _guideService;

        public GuideController(IGuideService guideService)
        {
            _guideService = guideService;
        }

        [Authorize]
        [HttpGet("get-list")]
        public async Task<IActionResult> GetListOfGuides()
        {
            var listOfGuides = await _guideService.ListOfGuidesAsync();
            return Ok(listOfGuides);
        }

        [Authorize(Roles = "GUIDE")]
        [HttpGet("progress-reports-list")]
        public async Task<IActionResult> ListOfProgressReportAssociatedWithGuide()
        {
            try
            {
                var result = await _guideService.GetProgressReportList();
                return Ok(new { resultData = result });
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
