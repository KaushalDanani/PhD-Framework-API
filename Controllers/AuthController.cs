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
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IStudentRepository studentRepository, IConfiguration configuration)
        {
            _authService = authService;
            _studentRepository = studentRepository;
            _configuration = configuration;
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
                return BadRequest(new { message = "Invalid sign-in model request" });

            try
            {
                var result = await _authService.SigninAsync(signinDto, confirmedEmail);
                return Ok(new { redirectURL = result.Message});
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

        [HttpPost("forgot-password-request")]
        public async Task<IActionResult> ForgotPasswordRequest([FromBody] ForgotPasswordRequestDto forgotPasswordRequest)
        {
            if (string.IsNullOrEmpty(forgotPasswordRequest.Email))
                return BadRequest(new {message = "Email address is required"});

            var result = await _authService.ForgotPasswordAsync(forgotPasswordRequest.Email);

            if (result.IsSuccess)
                return Ok(new {message = result.Message});

            return BadRequest(new {message = result.Message});
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordDto)
        {
            if (string.IsNullOrEmpty(resetPasswordDto.Token))
                return BadRequest(new { message = "Invalid token request"});

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(resetPasswordDto.Token, resetPasswordDto.Password);

            if (result.IsSuccess)
                return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
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
