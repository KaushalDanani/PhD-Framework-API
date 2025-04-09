using Backend.DTOs;
using Backend.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserFromTokenAsync();
        Task<StudentProfileDto> UpdateStudentAsync(ApplicationUser user, UserProfileUpdateDto newUserProfile);
        Task<GuideProfileDto> UpdateGuideAsync(ApplicationUser user, UserProfileUpdateDto newGuideProfile);
        Task<ServiceResponseDto> ChangeProfileImageAsync(IFormFile image, string email);
        Task<ApplicationUser> GetUserWithProfileImage(string userId);
    }
}
