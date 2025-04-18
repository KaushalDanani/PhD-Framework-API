using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicSectionController : ControllerBase
    {
        private readonly IGuideService _guideService;
        private readonly IDeanService _deanService;

        public AcademicSectionController(IGuideService guideService, IDeanService deanService)
        {
            _guideService = guideService;
            _deanService = deanService;
        }

        [HttpPost("add-guide")]
        public async Task<IActionResult> AddAuthorizedGuide([FromBody] AddGuideRequestDto guide)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid data model request" });

            ServiceResponseDto result;
            try
            {
                result = await _guideService.AddGuideAsync(guide);
                if (result.IsSuccess)
                    return Ok(new { message = "Guide added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }

            return BadRequest(new { message = result.Message });
        }

        [HttpPost("add-dean")]
        public async Task<IActionResult> AddAuthorizedDean([FromBody] AddDeanRequestDto dean)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid data model request" });

            ServiceResponseDto result;
            try
            {
                result = await _deanService.AddDeanAsync(dean);
                if (result.IsSuccess)
                    return Ok(new { message = "Dean added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }

            return BadRequest(new { message = result.Message });
        }
    }
}
