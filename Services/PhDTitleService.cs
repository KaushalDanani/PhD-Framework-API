using Backend.CustomExceptions;
using System.IdentityModel.Tokens.Jwt;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;
using Backend.Repositories;

namespace Backend.Services
{
    public class PhDTitleService : IPhDTitleService
    {
        private readonly IPhDTitleRepository _phDTitleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentRepository _studentRepository;

        public PhDTitleService(IPhDTitleRepository phDTitleRepository, IHttpContextAccessor contextAccessor, IStudentRepository studentRepository)
        {
            _phDTitleRepository = phDTitleRepository;
            _httpContextAccessor = contextAccessor;
            _studentRepository = studentRepository;
        }

        public (int year, int term) CalculatePhdYearAndTerm(DateTime startDate)
        {
            var today = DateTime.Today;

            // Total months since the start date
            int totalMonths = ((today.Year - startDate.Year) * 12) + today.Month - startDate.Month;

            if (today.Day < startDate.Day)
            {
                totalMonths--; // not yet completed the month
            }

            // Each term is 6 months
            int termNumber = totalMonths / 6;

            int year = (termNumber / 2) + 1;
            int term = (termNumber % 2 == 0) ? 1 : 2;

            return (year, term);
        }


        public async Task<bool> RegisterPhDTitle(PhDTitleRegistrationRequestDto phDTitleRegistrationDto)
        {
            if (phDTitleRegistrationDto.GuideId == phDTitleRegistrationDto.CoGuideId)
                return false;

            var (year, term) = CalculatePhdYearAndTerm(phDTitleRegistrationDto.StartDate);

            var phdTitle = new PhDTitle
            {
                RegistrationId = phDTitleRegistrationDto.PhDId,
                GuideId = phDTitleRegistrationDto.GuideId,
                CoGuideId = phDTitleRegistrationDto.CoGuideId,
                PhDTitleName = phDTitleRegistrationDto.PhdTitle,
                ResearchArea = phDTitleRegistrationDto.ResearchArea,
                StartDate = phDTitleRegistrationDto.StartDate,
                Status = true,
                Year = year,
                Term = term,
                ReRegistrationDate = null,
                LastUpdatedAt = DateTime.Now
            };
            await _phDTitleRepository.AddPhdTitle(phdTitle);
            return true;
        }

        public async Task<bool> IsPhDTitleRegisterAsync()
        {
            var token = _httpContextAccessor.HttpContext!.Request.Cookies["AuthToken"];
            //Console.WriteLine("Token:" + token);
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Invalid or expire token");

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userId = jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            //Console.WriteLine($"User Id: {userId}");

            if (string.IsNullOrEmpty(userId))
                throw new UserNotFoundException("Unauthorized User");

            // Get Student from userId which is basically get from jwt bearer
            var associatedStudent = await _studentRepository.GetStudentByUserIdAsync(userId);
            //Console.WriteLine($"## Student: {associatedStudent}");

            if (associatedStudent == null)
                throw new UnauthorizedAccessException("Unauthorized Student Access");

            var isRegister = await _phDTitleRepository.IsPhDTitleRecordFoundAsync(associatedStudent.RegistrationId);
            return isRegister;
        }
    }
}
