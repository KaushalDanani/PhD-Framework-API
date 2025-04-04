using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using Backend.CustomExceptions;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IdentityResult> SignupAsync(SignupRequestDto signupDto)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "STUDENT");
                if (role == null)
                    throw new Exception("The specified role does not exist.");

                var user = new ApplicationUser
                {
                    UserName = $"{signupDto.step1.FirstName} {signupDto.step1.LastName}",
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
                    RegistrationId = signupDto.step4.RegistrationNo,
                    UpdatedAt = DateTime.Now
                };

                _context.Students.Add(newStudent);
                await _context.SaveChangesAsync();

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink =
                    $"https://localhost:7197/api/Auth/confirm-email?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

                try
                {
                    await _emailService.SendEmailAsync(user.Email, confirmationLink,
                        $"{newStudent.FirstName} {newStudent.LastName}");
                }
                catch (Exception ex)
                {
                    _context.Students.Remove(newStudent);
                    await _userManager.DeleteAsync(user);
                    await _context.SaveChangesAsync();
                    await transaction.RollbackAsync();

                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "EmailSendingFailed",
                        Description = "Failed to send email confirmation. Please try again later."
                    });
                }

                await transaction.CommitAsync();
                return IdentityResult.Success;
            }
        }

        public async Task<IActionResult> SigninAsync(SigninRequestDto signinDto, string? confirmedEmail)
        {
            var user = await _userManager.FindByEmailAsync(signinDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, signinDto.Password))
                throw new UnauthorizedAccessException("Invalid email or password!");

            if(confirmedEmail != null && !"true".Equals(confirmedEmail))
                throw new EmailNotConfirmedException(
                "Please, confirm your email to activate your account. Email is already sent!");
            else if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new EmailNotConfirmedException(
                    "Please, confirm your email to activate your account. Email is already sent!");

            var jwtSettings = _configuration.GetSection("JWTSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var expiryTime = signinDto.RememberMe
                ? DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["LongTokenExpiry"]))
                : DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["TokenExpiry"]));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim("Role", user.ApplicationRoleId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                audience: jwtSettings["Audience"],
                claims: claims,
                signingCredentials: signingCredentials,
                expires: expiryTime 
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            { 
                throw new InvalidOperationException("HttpContext is not available.");
            }

            context.Response.Cookies.Append("AuthToken", tokenString, new CookieOptions 
            {
                    HttpOnly = true, // Prevent JavaScript access
                    Secure = true, // Ensures cookie is only sent over HTTPS
                    SameSite = SameSiteMode.None,
                    Expires = expiryTime
            });

            return new StatusCodeResult(200);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string token, string email)
        {
            var decodedEmail = Uri.UnescapeDataString(email);
            var user = await _userManager.FindByEmailAsync(decodedEmail);
            if (user == null)
                throw new Exception("User is not found!");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result;
        }

        public async Task<ServiceResponseDto> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ServiceResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found with the provided email address."
                };
            }

            if (!user.EmailConfirmed)
            {
                return new ServiceResponseDto
                {
                    IsSuccess = false,
                    Message = "Please activate your account by confirming email address through the email you received."
                };
            }

            // Generate Unique Password Reset Token
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var expirationTime = DateTime.Now.AddHours(2);

            // Store the reset token and expiration in the database
            var existingToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(rt => rt.Email == email);

            if (existingToken != null)
            {
                if (existingToken.ExpireTime > DateTime.Now)
                {
                    return new ServiceResponseDto
                    {
                        IsSuccess = false,
                        Message = "Password reset token already sent. Please check your email."
                    };
                }
                else
                {
                    // If a token already exists, update it
                    existingToken.Token = resetToken;
                    existingToken.ExpireTime = expirationTime;
                    _context.PasswordResetTokens.Update(existingToken);
                }
            }
            else
            {
                // If no token exists, insert a new one
                var newToken = new PasswordResetToken
                {
                    Email = email,
                    Token = resetToken,
                    ExpireTime = expirationTime
                };
                await _context.PasswordResetTokens.AddAsync(newToken);
            }
            await _context.SaveChangesAsync();

            // Send Reset Password Email
            var resetLink = $"http://localhost:4200/reset-password?token={Uri.EscapeDataString(resetToken)}";
            try
            {
                await _emailService.SendPasswordResetEmail(email, resetLink, user.UserName!);
            }
            catch (Exception ex)
            {
                return new ServiceResponseDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }

            return new ServiceResponseDto
            {
                IsSuccess = true,
                Message = "Password reset email sent successfully."
            };
        }

        public async Task<ServiceResponseDto> ResetPasswordAsync(string token, string newPassword)
        {
            var decodedToken = Uri.UnescapeDataString(token);
            var tokenRecord = await _context.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == decodedToken);

            if (tokenRecord == null || tokenRecord.ExpireTime < DateTime.Now)
            {
                return new ServiceResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid or expired token"
                };
            }

            var user = await _userManager.FindByEmailAsync(tokenRecord.Email);
            if (user == null)
            {
                return new ServiceResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);
            if (result.Succeeded)
            {
                _context.PasswordResetTokens.Remove(tokenRecord);
                await _context.SaveChangesAsync();

                return new ServiceResponseDto
                {
                    IsSuccess = true,
                    Message = "Password has been reset successfully"
                };
            }

            return new ServiceResponseDto
            {
                IsSuccess = false,
                Message = "Oops! New password didn't go through"
            };
        }
    }
}
