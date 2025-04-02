using Backend.CustomExceptions;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IStudentRepository _studentRepository;

        public AuthController(IAuthService authService, IStudentRepository studentRepository)
        {
            _authService = authService;
            _studentRepository = studentRepository;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequestDto signupDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.SignupAsync(signupDto);
                if (result.Succeeded)
                    return Ok(result);

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] SigninRequestDto signinDto, [FromQuery] string? confirmedEmail)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _authService.SigninAsync(signinDto, confirmedEmail);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (EmailNotConfirmedException exp)
            { 
                return BadRequest(new { message = exp.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Something went Wrong!", Details = ex.Message });
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
                return BadRequest("Invalid email confirmation request.");

            var result = await _authService.ConfirmEmailAsync(token, email);

            if (result.Succeeded)
                return Redirect("http://localhost:4200/signin?emailConfirmed=true");

            return BadRequest("Email confirmation failed!");
        }

        [Authorize]
        [HttpPost("sign-out")]
        public IActionResult Sign_out()
        {
            HttpContext.Response.Cookies.Delete("AuthToken");
            return Ok();
        }
    }
}
