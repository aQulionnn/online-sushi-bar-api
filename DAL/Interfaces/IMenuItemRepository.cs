using DAL.Entities;
using DAL.Parameters;

namespace DAL.Interfaces
{
    public interface IMenuItemRepository : IBaseRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> GetAllWithSortingAsync(SortingParameters sorting);
    }
}
