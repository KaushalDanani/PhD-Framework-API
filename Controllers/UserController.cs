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

        public UserController(IUserService userService, IStudentRepository studentRepository)
        {
            _userService = userService;
            _studentRepository = studentRepository;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            //Console.WriteLine("WELCOME TO FIRST STEP!!");
            var user = await _userService.GetUserFromTokenAsync();
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid or expired token." });
            }

            var role = user.Role.NormalizedName;

            if (role == "STUDENT")
            {
                var studentData = await _studentRepository.GetStudentByEmailAsync(user.Email!);

                var studentProfileData = new StudentProfileDto
                {
                    RegistrationNo = studentData.RegistrationId,
                    DOB = studentData.DateOfBirth.ToString("D"),
                    FirstName = studentData.FirstName,
                    LastName = studentData.LastName,
                    FatherName = studentData.FatherName,
                    Email = studentData.Email,
                    PhoneNo = studentData.PhoneNo,
                    Gender = studentData.Gender,
                    Department = studentData.Department,
                    Faculty = studentData.Faculty,
                    Address = studentData.Address,
                    City = studentData.City,
                    Pincode = studentData.Pincode,
                    State = studentData.State,
                    Country = studentData.Country,
                    UpdatedAt = studentData.UpdatedAt.ToString("F")
                };

                return Ok(new { userData = studentProfileData });
            }
            else
            {
                // Handle cases for other roles or profiles (if needed)
                return BadRequest(new { message = "Unsupported role for data access." });
            }
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateStudentProfile(string id, [FromBody] StudentProfileUpdateDto newStudent)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedStudent = await _userService.UpdateStudentAsync(id, newStudent);
                return Ok(new {userData = updatedStudent});
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
    }
}
