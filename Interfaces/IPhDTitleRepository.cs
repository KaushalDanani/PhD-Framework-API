using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IPhDTitleRepository
    {
        Task AddPhdTitle(PhDTitle phDTitle);
        Task<bool> GetPhDTitleRecordAsync(string phdId);
    }
}
