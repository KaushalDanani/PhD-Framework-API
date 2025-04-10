using Backend.Interfaces;
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

        [HttpGet("get-list")]
        public async Task<IActionResult> GetListOfGuides()
        {
            var listOfGuides = await _guideService.ListOfGuidesAsync();
            return Ok(listOfGuides);
        }

    }
}
