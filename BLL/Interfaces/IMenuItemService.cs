using BLL.Dtos.MenuItem;

namespace BLL.Interfaces
{
    public interface IMenuItemService 
    {
        Task<GetMenuItemDto> CreateAsync(CreateMenuItemDto createMenuItemDto);
        Task<IEnumerable<GetMenuItemDto>> GetAllAsync();
        Task<GetMenuItemDto> GetByIdAsync(int id);
        Task<GetMenuItemDto> UpdateAsync(int id, UpdateMenuItemDto updateMenuItemDto);
        Task<IEnumerable<GetMenuItemDto>> DeleteAllAsync();
        Task<GetMenuItemDto> DeleteByIdAsync(int id);
    }
}
