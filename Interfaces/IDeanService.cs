using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IDeanService
    {
        Task<ServiceResponseDto> AddDeanAsync(AddDeanRequestDto deanRequest);
    }
}
