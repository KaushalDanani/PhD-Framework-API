using Backend.DTOs;
using Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> SignupAsync(SignupRequestDto signupDto);
        Task<ServiceResponseDto> SigninAsync(SigninRequestDto signinDto, string? confirmedEmail);
        Task<IdentityResult> ConfirmEmailAsync(string token, string email);
        Task<ServiceResponseDto> ForgotPasswordAsync(string email);
        Task<ServiceResponseDto> ResetPasswordAsync(string token, string newPassword);
    }
}
