using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Parameters;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Queries
{
    public class GetSortedMenuItemsQueryHandler : IRequestHandler<GetSortedMenuItemsQuery, Result<IEnumerable<GetMenuItemDto>>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IRedisService _redisService;

        public GetSortedMenuItemsQueryHandler(IMenuItemService menuItemService, IRedisService redisService)
        {
            _menuItemService = menuItemService;
            _redisService = redisService;
        }

        public async Task<Result<IEnumerable<GetMenuItemDto>>> Handle(GetSortedMenuItemsQuery request, CancellationToken cancellationToken)
        {
            var cachedMenuItems = await _redisService.GetDataAsync<IEnumerable<GetMenuItemDto>>($"menuItems:sorted:{request.Sorting.GetHashCode()}");
            if (cachedMenuItems != null)
                return Result<IEnumerable<GetMenuItemDto>>.Success(cachedMenuItems);

            var menuItems = await _menuItemService.GetAllWithSortingAsync(request.Sorting);
            await _redisService.SetDataAsync($"menuItems:sorted:{request.Sorting.GetHashCode()}", menuItems);

            return Result<IEnumerable<GetMenuItemDto>>.Success(menuItems);
        }
    }

    public record GetSortedMenuItemsQuery(SortingParameters Sorting) : IRequest<Result<IEnumerable<GetMenuItemDto>>>;
}
