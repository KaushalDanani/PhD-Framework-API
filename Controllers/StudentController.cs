using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IPhDTitleService _phDTitleService;

        public StudentController(IPhDTitleService phDTitleService)
        {
            _phDTitleService = phDTitleService;
        }

        [Authorize]
        [HttpPost("phd-title-registration")]
        public async Task<IActionResult> PhDTitleRegistrationAsync(
            [FromBody] PhDTitleRegistrationRequestDto phDTitleRegistration)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid model request" });

            try
            {
                var isRegister = await _phDTitleService.RegisterPhDTitle(phDTitleRegistration);
                if (isRegister)
                    return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }

            return BadRequest(new { message = "PhD Title Registration failed" });
        }
    }
}
