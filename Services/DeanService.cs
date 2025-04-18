using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class DeanService : IDeanService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public DeanService(ApplicationDbContext context, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<ServiceResponseDto> AddDeanAsync(AddDeanRequestDto deanRequest)
        {
            Console.WriteLine("What is this Hell?!!");
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "DEAN OF FACULTY");
                if (role == null)
                {
                    return new ServiceResponseDto
                    {
                        IsSuccess = false,
                        Message = "Specified role does not exist"
                    };
                }

                var user = new ApplicationUser
                {
                    UserName = $"{deanRequest.FirstName} {deanRequest.LastName}",
                    Email = deanRequest.Email,
                    PhoneNumber = deanRequest.Phone,
                    ApplicationRoleId = role.Id,
                    EmailConfirmed = true,
                    ProfileImageId = Guid.Parse(_configuration["GoogleDrive:DefaultImageFileId"]!)
                };

                var result = await _userManager.CreateAsync(user, deanRequest.Password);
                if (!result.Succeeded)
                {
                    return new ServiceResponseDto
                    {
                        IsSuccess = false,
                        Message = "User isn't created"
                    };
                }

                var newDean = new Dean
                {
                    UserId = user.Id,
                    FirstName = deanRequest.FirstName,
                    LastName = deanRequest.LastName,
                    FatherName = deanRequest.FatherName,
                    Gender = deanRequest.Gender,
                    Email = deanRequest.Email,
                    DateOfBirth = deanRequest.Dob,
                    FacultyId = deanRequest.Faculty,
                    Address = deanRequest.Address,
                    City = deanRequest.City,
                    Pincode = deanRequest.Pincode,
                    State = deanRequest.State,
                    PhoneNo = deanRequest.Phone,
                    UpdatedAt = DateTime.Now
                };

                _context.Deans.Add(newDean);
                await _context.SaveChangesAsync();

                // Generate Unique Password Reset Token
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var expirationTime = DateTime.Now.AddHours(48);

                // If no token exists, insert a new one
                var newToken = new PasswordResetToken
                {
                    Email = deanRequest.Email,
                    Token = resetToken,
                    ExpireTime = expirationTime
                };
                _context.PasswordResetTokens.Add(newToken);
                await _context.SaveChangesAsync();

                // Send Reset Password Email
                var resetLink = $"http://localhost:4200/reset-password?token={Uri.EscapeDataString(resetToken)}";

                try
                {
                    await _emailService.SendTemporaryCredentialEmailAsync(deanRequest.Email, resetLink,
                        $"{deanRequest.FirstName} {deanRequest.LastName}", "Dean", deanRequest.Password);
                }
                catch (Exception ex)
                {
                    _context.PasswordResetTokens.Remove(newToken);
                    _context.Deans.Remove(newDean);
                    await _userManager.DeleteAsync(user);
                    await _context.SaveChangesAsync();
                    await transaction.RollbackAsync();

                    return new ServiceResponseDto
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    };
                }

                await transaction.CommitAsync();
                return new ServiceResponseDto
                {
                    IsSuccess = true
                };
            }
        }
    }
}
