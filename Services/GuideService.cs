using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class GuideService : IGuideService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IGuideRepository _guideRepository;

        public GuideService(ApplicationDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager, IEmailService emailService, IGuideRepository guideRepository)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _emailService = emailService;
            _guideRepository = guideRepository;
        }

        public async Task<ServiceResponseDto> AddGuideAsync(AddGuideRequestDto guideRequest)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "GUIDE");
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
                    UserName = $"{guideRequest.FirstName} {guideRequest.LastName}",
                    Email = guideRequest.Email,
                    PhoneNumber = guideRequest.Phone,
                    ApplicationRoleId = role.Id,
                    EmailConfirmed = true,
                    ProfileImageId = Guid.Parse(_configuration["GoogleDrive:DefaultImageFileId"]!)
                };

                var result = await _userManager.CreateAsync(user, guideRequest.Password);
                if (!result.Succeeded)
                {
                    return new ServiceResponseDto
                    {
                        IsSuccess = false,
                        Message = "User isn't created"
                    };
                }

                var newGuide = new Guide
                {
                    UserId = user.Id,
                    FirstName = guideRequest.FirstName,
                    LastName = guideRequest.LastName,
                    FatherName = guideRequest.FatherName,
                    Gender = guideRequest.Gender,
                    Email = guideRequest.Email,
                    DateOfBirth = guideRequest.Dob,
                    DepartmentName = guideRequest.Department,
                    FacultyName = guideRequest.Faculty,
                    Address = guideRequest.Address,
                    City = guideRequest.City,
                    PinCode = guideRequest.Pincode,
                    State = guideRequest.State,
                    Country = guideRequest.Country,
                    ContactNo = guideRequest.Phone,
                    Designation = guideRequest.Designation,
                    Specialist = guideRequest.Specialist,
                    StudentsLimit = guideRequest.StudentLimit,
                    ExperienceYears = guideRequest.YearsOfExperience,
                    UpdatedAt = DateTime.Now
                };

                _context.Guides.Add(newGuide);
                await _context.SaveChangesAsync();

                // Generate Unique Password Reset Token
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var expirationTime = DateTime.Now.AddHours(48);

                // If no token exists, insert a new one
                var newToken = new PasswordResetToken
                {
                    Email = guideRequest.Email,
                    Token = resetToken,
                    ExpireTime = expirationTime
                };
                _context.PasswordResetTokens.Add(newToken);
                await _context.SaveChangesAsync();

                // Send Reset Password Email
                var resetLink = $"http://localhost:4200/reset-password?token={Uri.EscapeDataString(resetToken)}";

                try
                {
                    await _emailService.SendTemporaryCredentialEmailAsync(guideRequest.Email, resetLink,
                        $"{guideRequest.FirstName} {guideRequest.LastName}", "Guide", guideRequest.Password);
                }
                catch (Exception ex)
                {
                    _context.PasswordResetTokens.Remove(newToken);
                    _context.Guides.Remove(newGuide);
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

        public async Task<List<GetGuideListResponseDto>> ListOfGuidesAsync()
        {
            return await _guideRepository.GetAllGuideAsListAsync();
        }
    }
}
