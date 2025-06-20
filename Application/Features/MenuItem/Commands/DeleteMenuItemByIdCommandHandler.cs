using Application.Delegates;
using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Enums;
using DAL.Errors;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Commands
{
    public class DeleteMenuItemByIdCommandHandler : IRequestHandler<DeleteMenuItemByIdCommand, Result<GetMenuItemDto>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly CacheServiceResolver  _cacheServiceResolver;
        public DeleteMenuItemByIdCommandHandler(IMenuItemService menuItemService, CacheServiceResolver cacheServiceResolver)
        {
            _menuItemService = menuItemService;
            _cacheServiceResolver = cacheServiceResolver;
        }

        public async Task<Result<GetMenuItemDto>> Handle(DeleteMenuItemByIdCommand request, CancellationToken cancellationToken)
        {
            var redisCacheService = _cacheServiceResolver(CachingType.Redis);
            
            var menuItem = await _menuItemService.DeleteByIdAsync(request.id);
            if (menuItem == null) 
                return Result<GetMenuItemDto>.Failure(MenuItemErrors.NotFound);

            await redisCacheService.DeleteDataAsync($"menuItem:{request.id}");

            return Result<GetMenuItemDto>.Success(menuItem);
        }
    }
    public record DeleteMenuItemByIdCommand(int id) : IRequest<Result<GetMenuItemDto>> { }
}
