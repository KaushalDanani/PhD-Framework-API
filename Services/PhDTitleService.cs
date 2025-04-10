using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;

namespace Backend.Services
{
    public class PhDTitleService : IPhDTitleService
    {
        private readonly IPhDTitleRepository _phDTitleRepository;

        public PhDTitleService(IPhDTitleRepository phDTitleRepository)
        {
            _phDTitleRepository = phDTitleRepository;
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
    }
}
