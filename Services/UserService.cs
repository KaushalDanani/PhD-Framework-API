using Backend.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Backend.CustomExceptions;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using Backend.DTOs;
using Backend.Data;
using Google.Apis.Drive.v3.Data;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IStudentRepository _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IApplicationFileRepository _fileRepository;
        private readonly IConfiguration _configuration;
        private readonly IGuideRepository _guideRepository;

        public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IStudentRepository studentRepository, IHttpContextAccessor contextAccessor, IGoogleDriveService googleDriveService, IApplicationFileRepository fileRepository, IConfiguration configuration, IGuideRepository guideRepository)
        {
            _userManager = userManager;
            _context = dbContext;
            _studentRepository = studentRepository;
            _httpContextAccessor = contextAccessor;
            _googleDriveService = googleDriveService;
            _fileRepository = fileRepository;
            _configuration = configuration;
            _guideRepository = guideRepository;
        }

        public async Task<ApplicationUser?> GetUserFromTokenAsync()
        {
            var token = _httpContextAccessor.HttpContext!.Request.Cookies["AuthToken"];
            //Console.WriteLine("Token:" + token);
            if (string.IsNullOrEmpty(token))
            {
                return null!;
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userId = jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (userId == null)
            {
                return null!;
            }

            var user = await _userManager.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user!;
        }

        public async Task<ApplicationUser> GetUserWithProfileImage(string userId)
        {
            return (await _context.Users
                .Include(nav => nav.ProfileImage)
                .FirstOrDefaultAsync(u => u.Id == userId))!;
        }

        public async Task<StudentProfileDto> UpdateStudentAsync(ApplicationUser user, UserProfileUpdateDto newUserProfile)
        {
            var existingStudent = await _studentRepository.GetStudentByUserIdAsync(user.Id);
            if (existingStudent == null)
            { ;
                throw new UserNotFoundException("Student not found!");
            }

            var userWithProfileImage = await GetUserWithProfileImage(user.Id);

            // Updated fields to existing entity
            if (!string.IsNullOrEmpty(newUserProfile.FirstName))
                existingStudent.FirstName = newUserProfile.FirstName;
            if (!string.IsNullOrEmpty(newUserProfile.LastName))
                existingStudent.LastName = newUserProfile.LastName;
            if (!string.IsNullOrEmpty(newUserProfile.FatherName))
                existingStudent.FatherName = newUserProfile.FatherName;
            if (!string.IsNullOrEmpty(newUserProfile.PhoneNo))
                existingStudent.PhoneNo = newUserProfile.PhoneNo;
            if (!string.IsNullOrEmpty(newUserProfile.Address))
                existingStudent.Address = newUserProfile.Address;
            if (!string.IsNullOrEmpty(newUserProfile.Gender))
                existingStudent.Gender = newUserProfile.Gender;
            if (!string.IsNullOrEmpty(newUserProfile.City))
                existingStudent.City = newUserProfile.City;
            existingStudent.DateOfBirth = newUserProfile.Dob;
            if (!string.IsNullOrEmpty(newUserProfile.State))
                existingStudent.State = newUserProfile.State;
            if (!string.IsNullOrEmpty(newUserProfile.Pincode))
                existingStudent.Pincode = newUserProfile.Pincode;
            if (!string.IsNullOrEmpty(newUserProfile.Country))
                existingStudent.Country = newUserProfile.Country;
            existingStudent.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Profile isn't updated!");
            }

            var studentWithFacultyAndDepartment =
                await _studentRepository.GetStudentWithFacultyAndDepartmentAsync(existingStudent.Email);

            var studentDto = new StudentProfileDto
            {
                RegistrationNo = existingStudent.RegistrationId,
                ProfileImageUrl = userWithProfileImage.ProfileImage.FilePath,
                FirstName = existingStudent.FirstName,
                LastName = existingStudent.LastName,
                FatherName = existingStudent.FatherName,
                Email = existingStudent.Email,
                PhoneNo = existingStudent.PhoneNo,
                Address = existingStudent.Address,
                Gender = existingStudent.Gender,
                City = existingStudent.City,
                DOB = existingStudent.DateOfBirth.ToLongDateString(),
                Department = studentWithFacultyAndDepartment.Department.Name,
                Faculty = studentWithFacultyAndDepartment.Faculty.Name,
                State = existingStudent.State,
                Pincode = existingStudent.Pincode,
                Country = existingStudent.Country,
                UpdatedAt = existingStudent.UpdatedAt.ToString()
            };

            return studentDto;
        }

        public async Task<GuideProfileDto> UpdateGuideAsync(ApplicationUser user, UserProfileUpdateDto newGuideProfile)
        {
            var existingGuide = await _guideRepository.GetGuideByUserIdAsync(user.Id);
            if (existingGuide == null)
            {
                throw new UserNotFoundException("Guide not found!");
            }

            var userWithProfileImage = await GetUserWithProfileImage(user.Id);

            // Updated fields to existing entity
            if (!string.IsNullOrEmpty(newGuideProfile.FirstName))
                existingGuide.FirstName = newGuideProfile.FirstName;
            if (!string.IsNullOrEmpty(newGuideProfile.LastName))
                existingGuide.LastName = newGuideProfile.LastName;
            if (!string.IsNullOrEmpty(newGuideProfile.FatherName))
                existingGuide.FatherName = newGuideProfile.FatherName;
            if (!string.IsNullOrEmpty(newGuideProfile.PhoneNo))
                existingGuide.ContactNo = newGuideProfile.PhoneNo;
            if (!string.IsNullOrEmpty(newGuideProfile.Address))
                existingGuide.Address = newGuideProfile.Address;
            if (!string.IsNullOrEmpty(newGuideProfile.Gender))
                existingGuide.Gender = newGuideProfile.Gender;
            if (!string.IsNullOrEmpty(newGuideProfile.City))
                existingGuide.City = newGuideProfile.City;
            existingGuide.DateOfBirth = newGuideProfile.Dob;
            if (!string.IsNullOrEmpty(newGuideProfile.State))
                existingGuide.State = newGuideProfile.State;
            if (!string.IsNullOrEmpty(newGuideProfile.Pincode))
                existingGuide.PinCode = newGuideProfile.Pincode;
            if (!string.IsNullOrEmpty(newGuideProfile.Country))
                existingGuide.Country = newGuideProfile.Country;
            if (!string.IsNullOrEmpty(newGuideProfile.Specialist))
                existingGuide.Specialist = newGuideProfile.Specialist;
            existingGuide.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Profile isn't updated!");
            }

            var guideDto = new GuideProfileDto
            {
                ProfileImageUrl = userWithProfileImage.ProfileImage.FilePath,
                FirstName = existingGuide.FirstName,
                LastName = existingGuide.LastName,
                FatherName = existingGuide.FatherName,
                Email = existingGuide.Email,
                PhoneNo = existingGuide.ContactNo,
                Address = existingGuide.Address,
                Gender = existingGuide.Gender,
                City = existingGuide.City,
                DOB = existingGuide.DateOfBirth.ToLongDateString(),
                Department = existingGuide.Department.Name,
                Faculty = existingGuide.Faculty.Name,
                State = existingGuide.State,
                Pincode = existingGuide.PinCode,
                Country = existingGuide.Country,
                Designation = existingGuide.Designation,
                Specialist = existingGuide.Specialist,
                StudentLimit = existingGuide.StudentsLimit,
                ExperienceYears = existingGuide.ExperienceYears,
                UpdatedAt = existingGuide.UpdatedAt.ToString()
            };

            return guideDto;
        }

        public async Task<ServiceResponseDto> ChangeProfileImageAsync(IFormFile image, string email)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".heic" };
            var fileExtension = Path.GetExtension(image.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return new ServiceResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid file type."
                };
            }

            // Upload image to Google Drive
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
                var userWithProfileImage = await GetUserWithProfileImage(user.Id);
                string fileName;
                if (user == null)
                    fileName = image.FileName;
                else
                    fileName = "PI-" + user.Email?.Split("@")[0];

                var imageFolderId = _configuration["GoogleDrive:ImageFolderId"]!;

                if (user != null && user.IsProfileImageChanged)
                {
                    var isDeleteSuccess = await _googleDriveService.DeleteFileFromDriveAsync(userWithProfileImage.ProfileImage.StoredCloudFileId);

                    if (!isDeleteSuccess)
                    {
                        return new ServiceResponseDto
                        {
                            IsSuccess = false,
                            Message = "Failed to upload new profile image"
                        };
                    }

                    using (var fileStream = image.OpenReadStream())
                    {
                        var fileData = await _googleDriveService.UploadFileToDriveAsync(fileStream, fileName, image.ContentType, imageFolderId);

                        var existingApplicationFile = await _fileRepository.GetByFileIdAsync(user.ProfileImageId);
                        if (existingApplicationFile != null)
                        {
                            // Update the fields of the existing file record
                            existingApplicationFile.StoredCloudFileId = fileData.ResponseBody.Id;
                            existingApplicationFile.MimeType = image.ContentType;
                            existingApplicationFile.FileSize = fileData.ResponseBody.Size ?? 0;
                            existingApplicationFile.FilePath = $"https://drive.google.com/thumbnail?id={fileData.ResponseBody.Id}"; // Google Drive link
                            existingApplicationFile.UploadedAt = DateTime.Now;

                            // Save the updated application file record to the database
                            await _fileRepository.UpdateAsync(existingApplicationFile);

                            return new ServiceResponseDto
                            {
                                IsSuccess = true,
                                Message = existingApplicationFile.FilePath
                            };
                        }
                    }

                    return new ServiceResponseDto
                    {
                        IsSuccess = false,
                        Message = "Failed to update image"
                    };
                }
                else
                {
                    using (var fileStream = image.OpenReadStream())
                    {
                        var fileData = await _googleDriveService.UploadFileToDriveAsync(fileStream, fileName, image.ContentType, imageFolderId);

                        // Create an ApplicationFile entity to store in the database
                        var applicationFile = new ApplicationFile
                        {
                            FileId = Guid.NewGuid(),
                            StoredCloudFileId = fileData.ResponseBody.Id,
                            RelatedTable = "ApplicationUser",
                            FileName = fileName,
                            MimeType = image.ContentType,
                            FileSize = fileData.ResponseBody.Size ?? 0, // If file size is null, default to 0
                            FilePath = $"https://drive.google.com/thumbnail?id={fileData.ResponseBody.Id}", // Google Drive link
                            UploadedAt = DateTime.Now
                        };

                        await _fileRepository.AddAsync(applicationFile);

                        user.ProfileImageId = applicationFile.FileId;
                        user.IsProfileImageChanged = true;
                        await _userManager.UpdateAsync(user); // Save user with new ProfileImageId

                        return new ServiceResponseDto
                        {
                            IsSuccess = true,
                            Message = applicationFile.FilePath
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponseDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
