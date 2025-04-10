using Backend.Entities;

namespace Backend.Interfaces
{
    public interface IPhDTitleRepository
    {
        Task AddPhdTitle(PhDTitle phDTitle);
    }
}
