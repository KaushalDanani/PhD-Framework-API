using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IdentityResult> SignupAsync(SignupRequestDto signupDto)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "STUDENT");
            if (role == null)
                throw new Exception("The specified role does not exist.");

            var user = new ApplicationUser
            { 
                UserName = signupDto.step1.Email,
                Email = signupDto.step1.Email,
                PhoneNumber = signupDto.step1.Phone,
                ApplicationRoleId = role.Id
            };

            var result = await _userManager.CreateAsync(user, signupDto.step4.Password);
            if (!result.Succeeded) return result;

            var newStudent = new Student
            {
                UserId = user.Id,
                FirstName = signupDto.step1.FirstName,
                LastName = signupDto.step1.LastName,
                FatherName = signupDto.step1.FatherName,
                Email = signupDto.step1.Email,
                PhoneNo = signupDto.step1.Phone,
                DateOfBirth = signupDto.step2.Dob,
                Gender = signupDto.step2.Gender,
                Department = signupDto.step2.Department,
                Faculty = signupDto.step2.Faculty,
                Address = signupDto.step3.Address,
                City = signupDto.step3.City,
                Pincode = signupDto.step3.Pincode,
                State = signupDto.step3.State,
                Country = signupDto.step3.Country,
                RegistrationId = signupDto.step4.RegistrationNo
            };

            _context.Students.Add(newStudent);
            await _context.SaveChangesAsync();

            return IdentityResult.Success;
        }
    }
}
