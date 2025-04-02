using System.Globalization;
using Backend.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Backend.CustomExceptions;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using Backend.DTOs;
using Backend.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IStudentRepository _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IStudentRepository studentRepository, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _context = dbContext;
            _studentRepository = studentRepository;
            _httpContextAccessor = contextAccessor;
        }

        public async Task<ApplicationUser?> GetUserFromTokenAsync()
        {
            var token = _httpContextAccessor.HttpContext!.Request.Cookies["AuthToken"];
            Console.WriteLine("Token:" + token);
            if (string.IsNullOrEmpty(token))
            {
                return null!;
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userId = jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            Console.WriteLine("User ID:" + userId);

            if (userId == null)
            {
                return null!;
            }

            var user = await _userManager.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
            Console.WriteLine("User Data:" + user);
            return user!;
        }

        public async Task<StudentProfileDto> UpdateStudentAsync(string registrationNo, StudentProfileUpdateDto newStudentProfile)
        {
            var existingStudent = await _studentRepository.GetStudentByRegistrationIdAsync(registrationNo);

            if (existingStudent == null)
            { ;
                throw new UserNotFoundException("Student not found!");
            }

            // Updated fields to existing entity
            if (!string.IsNullOrEmpty(newStudentProfile.FirstName))
                existingStudent.FirstName = newStudentProfile.FirstName;
            if (!string.IsNullOrEmpty(newStudentProfile.LastName))
                existingStudent.LastName = newStudentProfile.LastName;
            if (!string.IsNullOrEmpty(newStudentProfile.FatherName))
                existingStudent.FatherName = newStudentProfile.FatherName;
            if (!string.IsNullOrEmpty(newStudentProfile.PhoneNo))
                existingStudent.PhoneNo = newStudentProfile.PhoneNo;
            if (!string.IsNullOrEmpty(newStudentProfile.Address))
                existingStudent.Address = newStudentProfile.Address;
            if (!string.IsNullOrEmpty(newStudentProfile.Gender))
                existingStudent.Gender = newStudentProfile.Gender;
            if (!string.IsNullOrEmpty(newStudentProfile.City))
                existingStudent.City = newStudentProfile.City;
            existingStudent.DateOfBirth = newStudentProfile.Dob;
            if (!string.IsNullOrEmpty(newStudentProfile.State))
                existingStudent.State = newStudentProfile.State;
            if (!string.IsNullOrEmpty(newStudentProfile.Pincode))
                existingStudent.Pincode = newStudentProfile.Pincode;
            if (!string.IsNullOrEmpty(newStudentProfile.Country))
                existingStudent.Country = newStudentProfile.Country;
            existingStudent.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Database isn't updated!");
            }

            var studentDto = new StudentProfileDto
            {
                RegistrationNo = existingStudent.RegistrationId,
                FirstName = existingStudent.FirstName,
                LastName = existingStudent.LastName,
                FatherName = existingStudent.FatherName,
                Email = existingStudent.Email,
                PhoneNo = existingStudent.PhoneNo,
                Address = existingStudent.Address,
                Gender = existingStudent.Gender,
                City = existingStudent.City,
                DOB = existingStudent.DateOfBirth.ToLongDateString(),
                Department = existingStudent.Department,
                Faculty = existingStudent.Faculty,
                State = existingStudent.State,
                Pincode = existingStudent.Pincode,
                Country = existingStudent.Country,
                UpdatedAt = existingStudent.UpdatedAt.ToString()
            };

            return studentDto;
        }
    }
}
