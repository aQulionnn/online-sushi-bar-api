using Application.Delegates;
using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Enums;
using DAL.Errors;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Queries
{
    public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, Result<GetMenuItemDto>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly CacheServiceResolver  _cacheServiceResolver;
        public GetMenuItemByIdQueryHandler(IMenuItemService menuItemService, CacheServiceResolver cacheServiceResolver)
        {
            _menuItemService = menuItemService;
            _cacheServiceResolver = cacheServiceResolver;
        }

        public async Task<Result<GetMenuItemDto>> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
        {
            var redisCacheService = _cacheServiceResolver(CachingType.Redis);
            
            var cachedMenuItem = await redisCacheService.GetDataAsync<GetMenuItemDto>($"menuItem:{request.id}");
            if (cachedMenuItem != null)
                return Result<GetMenuItemDto>.Success(cachedMenuItem);

            var menuItem = await _menuItemService.GetByIdAsync(request.id);
            if (menuItem == null) 
                return Result<GetMenuItemDto>.Failure(MenuItemErrors.NotFound);

            await redisCacheService.SetDataAsync($"menuItem:{request.id}", menuItem);

            return Result<GetMenuItemDto>.Success(menuItem);
        }
    }

    public record GetMenuItemByIdQuery(int id) : IRequest<Result<GetMenuItemDto>> { }
}
