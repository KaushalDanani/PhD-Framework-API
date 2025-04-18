using Backend.CustomExceptions;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStudentRepository _studentRepository;
        private readonly IGuideRepository _guideRepository;

        public UserController(IUserService userService, IStudentRepository studentRepository, IGuideRepository guideRepository)
        {
            _userService = userService;
            _studentRepository = studentRepository;
            _guideRepository = guideRepository;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var user = await _userService.GetUserFromTokenAsync();
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid or expired token." });
            }

            var role = user.Role.NormalizedName;

            var userWithProfileImage = await _userService.GetUserWithProfileImage(user.Id);

            if (role == "STUDENT")
            {
                var studentData = await _studentRepository.GetStudentWithFacultyAndDepartmentAsync(user.Email!);

                var studentProfileData = new StudentProfileDto
                {
                    RegistrationNo = studentData.RegistrationId,
                    ProfileImageUrl = userWithProfileImage.ProfileImage.FilePath,
                    DOB = studentData.DateOfBirth.ToString("D"),
                    FirstName = studentData.FirstName,
                    LastName = studentData.LastName,
                    FatherName = studentData.FatherName,
                    Email = studentData.Email,
                    PhoneNo = studentData.PhoneNo,
                    Gender = studentData.Gender,
                    Department = studentData.Department.Name,
                    Faculty = studentData.Faculty.Name,
                    Address = studentData.Address,
                    City = studentData.City,
                    Pincode = studentData.Pincode,
                    State = studentData.State,
                    Country = studentData.Country,
                    UpdatedAt = studentData.UpdatedAt.ToString("F")
                };

                return Ok(new { userData = studentProfileData });
            }
            else if (role == "GUIDE")
            {
                var guide = await _guideRepository.GetGuideWithFacultyAndDepartmentAsync(user.Email!);

                var guideProfileData = new GuideProfileDto
                {
                    ProfileImageUrl = userWithProfileImage.ProfileImage.FilePath,
                    DOB = guide.DateOfBirth.ToString("D"),
                    FirstName = guide.FirstName,
                    LastName = guide.LastName,
                    FatherName = guide.FatherName,
                    Email = guide.Email,
                    PhoneNo = guide.ContactNo,
                    Gender = guide.Gender,
                    Department = guide.Department.Name,
                    Faculty = guide.Faculty.Name,
                    Address = guide.Address,
                    City = guide.City,
                    Pincode = guide.PinCode,
                    State = guide.State,
                    Country = guide.Country,
                    Designation = guide.Designation,
                    ExperienceYears = guide.ExperienceYears,
                    Specialist = guide.Specialist,
                    StudentLimit = guide.StudentsLimit,
                    UpdatedAt = guide.UpdatedAt.ToString("F")
                };

                return Ok(new { userData = guideProfileData });
            }
            else
            {
                // Handle cases for other roles or profiles (if needed)
                return BadRequest(new { message = "Unsupported role for data access." });
            }
        }

        [Authorize]
        [HttpPatch("update-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileUpdateDto newUserData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.GetUserFromTokenAsync();
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid or expired Authorization token." });
            }

            var role = user.Role.NormalizedName;

            if (role == "STUDENT")
            {
                try
                {
                    var updatedStudent = await _userService.UpdateStudentAsync(user, newUserData);
                    return Ok(new { userData = updatedStudent });
                }
                catch (UserNotFoundException ex)
                {
                    return NotFound("Student doesn't exist!");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Server Error", details = ex.Message });
                }
            }
            else if (role == "GUIDE")
            {
                try
                {
                    var updatedGuide = await _userService.UpdateGuideAsync(user, newUserData);
                    return Ok(new { userData = updatedGuide });
                }
                catch (UserNotFoundException ex)
                {
                    return NotFound("Guide doesn't exist!");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Server Error", details = ex.Message });
                }
            }

            return BadRequest(new { message = "Server Error" });
        }

        [Authorize]
        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile image, [FromForm] string email)
        {
            if (image.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }

            if (image.Length > 2 * 1024 * 1024)
            {
                return BadRequest(new { message = "File size is too large. Maximum allowed size is 2MB." });
            }

            var result = await _userService.ChangeProfileImageAsync(image, email);
            if(result.IsSuccess)
                return Ok(new { message = result.Message });

            return BadRequest(new { message = "Image isn't uploaded, Try again later!" });
        }
    }
}
