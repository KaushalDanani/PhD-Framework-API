using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IPhDTitleService
    {
        Task<bool> RegisterPhDTitle(PhDTitleRegistrationRequestDto  phDTitleRegistrationDto);
        (int year, int term) CalculatePhdYearAndTerm(DateTime startDate);
        Task<bool> IsPhDTitleRegisterAsync();
    }
}
