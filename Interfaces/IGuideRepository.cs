using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IGuideRepository
    {
        Task<Guide> GetGuideByEmailAsync(string email);
    }
}
