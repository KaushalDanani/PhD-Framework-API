using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IGuideService
    {
        Task<ServiceResponseDto> AddGuideAsync(AddGuideRequestDto guideRequest);
        Task<List<GetGuideListResponseDto>> ListOfGuidesAsync();
        Task<List<ProgressReportRequestsMetaDataDto>> GetProgressReportList();
        Task<ServiceResponseDto> UpdatedProgressReportsReviewStatus(ProgressReportReviewDto reportReview);
    }
}
