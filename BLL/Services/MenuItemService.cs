using AutoMapper;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Parameters;

namespace BLL.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MenuItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetMenuItemDto> CreateAsync(CreateMenuItemDto createMenuItemDto)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var menuItem = _mapper.Map<MenuItem>(createMenuItemDto);
                var createdMenuItem = await _unitOfWork.MenuItemRepository.CreateAsync(menuItem);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<GetMenuItemDto>(createdMenuItem);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<GetMenuItemDto>> DeleteAllAsync()
        {
            var deletedMenuItems = _mapper.Map<IEnumerable<GetMenuItemDto>>(await _unitOfWork.MenuItemRepository.DeleteAllAsync());
            return deletedMenuItems;
        }

        public async Task<GetMenuItemDto> DeleteByIdAsync(int id)
        {
            var deletedMenuItem = _mapper.Map<GetMenuItemDto>(await _unitOfWork.MenuItemRepository.DeleteByIdAsync(id));
            return deletedMenuItem;
        }

        public async Task<IEnumerable<GetMenuItemDto>> GetAllAsync(PaginationParameters pagination)
        {
            var menuItems = _mapper.Map<IEnumerable<GetMenuItemDto>>(await _unitOfWork.MenuItemRepository.GetAllAsync(pagination));
            return menuItems;
        }

        public async Task<IEnumerable<GetMenuItemDto>> GetAllWithSortingAsync(SortingParameters sorting)
        {
            var menuItems = _mapper.Map<IEnumerable<GetMenuItemDto>>(await _unitOfWork.MenuItemRepository.GetAllWithSortingAsync(sorting));
            return menuItems;
        }

        public async Task<GetMenuItemDto> GetByIdAsync(int id)
        {
            var menuItem = _mapper.Map<GetMenuItemDto>(await _unitOfWork.MenuItemRepository.GetByIdAsync(id));
            return menuItem;
        }

        public async Task<GetMenuItemDto> UpdateAsync(int id, UpdateMenuItemDto updateMenuItemDto)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var updatedMenuItem = _mapper.Map<MenuItem>(updateMenuItemDto);
                updatedMenuItem.Id = id;

                var menuItem = await _unitOfWork.MenuItemRepository.UpdateAsync(id, updatedMenuItem);
                if (menuItem == null)
                    return null;
                
                await _unitOfWork.CommitAsync();

                return _mapper.Map<GetMenuItemDto>(updatedMenuItem);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}
