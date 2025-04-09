using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IGuideService
    {
        Task<ServiceResponseDto> AddGuideAsync(AddGuideRequestDto guideRequest);
    }
}
