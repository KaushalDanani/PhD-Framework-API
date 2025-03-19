using Backend.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Backend.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> SignupAsync(SignupRequestDto signupDto);
    }
}
