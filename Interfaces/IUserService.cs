using Backend.DTOs;
using Backend.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserFromTokenAsync();
        Task<StudentProfileDto> UpdateStudentAsync(string registrationNo, StudentProfileUpdateDto newStudentProfile);
        Task<ServiceResponseDto> ChangeProfileImageAsync(IFormFile image, string email);
        Task<ApplicationUser> GetUserWithProfileImage(string userId);
    }
}
