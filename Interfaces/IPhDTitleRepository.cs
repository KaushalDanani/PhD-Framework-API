using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IPhDTitleRepository
    {
        Task AddPhdTitle(PhDTitle phDTitle);
        Task<bool> IsPhDTitleRecordFoundAsync(string phdId);
        Task<PhDTitle> GetPhDTitleAsync(string phdId);
    }
}
