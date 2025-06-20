using Application.Delegates;
using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Enums;
using DAL.Parameters;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Queries
{
    public class GetAllMenuItemQueryHandler : IRequestHandler<GetAllMenuItemQuery, Result<IEnumerable<GetMenuItemDto>>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly CacheServiceResolver  _cacheServiceResolver;
        public GetAllMenuItemQueryHandler(IMenuItemService menuItemService, CacheServiceResolver cacheServiceResolver)
        {
            _menuItemService = menuItemService;
            _cacheServiceResolver = cacheServiceResolver;
        }

        public async Task<Result<IEnumerable<GetMenuItemDto>>> Handle(GetAllMenuItemQuery request, CancellationToken cancellationToken)
        {
            var redisCacheService = _cacheServiceResolver(CachingType.Redis);
            
            var cachedMenuItems = await redisCacheService.GetDataAsync<IEnumerable<GetMenuItemDto>>
                ($"menuItems:page:{request.Pagination.Page}:pageSize:{request.Pagination.PageSize}");

            if (cachedMenuItems != null)
                return Result<IEnumerable<GetMenuItemDto>>.Success(cachedMenuItems);

            var menuItems = await _menuItemService.GetAllAsync(request.Pagination);
            await redisCacheService.SetDataAsync($"menuItems:page:{request.Pagination.Page}:pageSize:{request.Pagination.PageSize}", menuItems);

            return Result<IEnumerable<GetMenuItemDto>>.Success(menuItems);
        }
    }

    public record GetAllMenuItemQuery(PaginationParameters Pagination) : IRequest<Result<IEnumerable<GetMenuItemDto>>> { }
}
