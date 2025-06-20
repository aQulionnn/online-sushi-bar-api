using Application.Delegates;
using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Enums;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Commands
{
    public class DeleteAllMenuItemCommandHandler : IRequestHandler<DeleteAllMenuItemCommand, Result<IEnumerable<GetMenuItemDto>>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly CacheServiceResolver  _cacheServiceResolver;

        public DeleteAllMenuItemCommandHandler(IMenuItemService menuItemService, CacheServiceResolver cacheServiceResolver)
        {
            _menuItemService = menuItemService;
            _cacheServiceResolver = cacheServiceResolver;
        }

        public async Task<Result<IEnumerable<GetMenuItemDto>>> Handle(DeleteAllMenuItemCommand request, CancellationToken cancellationToken)
        {
            var redisCacheService = _cacheServiceResolver(CachingType.Redis);
            
            var menuItems = await _menuItemService.DeleteAllAsync();
            await redisCacheService.DeleteDataAsync("menuItems");
            return Result<IEnumerable<GetMenuItemDto>>.Success(menuItems);
        }
    }

    public record DeleteAllMenuItemCommand() : IRequest<Result<IEnumerable<GetMenuItemDto>>> { }
}
