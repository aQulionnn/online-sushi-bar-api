using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using MediatR;

namespace Application.Features.MenuItem.Queries
{
    public class GetAllMenuItemQueryHandler : IRequestHandler<GetAllMenuItemQuery, IEnumerable<GetMenuItemDto>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IRedisService _redisService;
        public GetAllMenuItemQueryHandler(IMenuItemService menuItemService, IRedisService redisService)
        {
            _menuItemService = menuItemService;
            _redisService = redisService;
        }

        public async Task<IEnumerable<GetMenuItemDto>> Handle(GetAllMenuItemQuery request, CancellationToken cancellationToken)
        {
            var cachedMenuItems = await _redisService.GetDataAsync<IEnumerable<GetMenuItemDto>>("menuItems");
            if (cachedMenuItems != null)
                return cachedMenuItems;

            var menuItems = await _menuItemService.GetAllAsync();
            await _redisService.SetDataAsync("menuItems", menuItems);
            return menuItems;
        }
    }

    public record GetAllMenuItemQuery() : IRequest<IEnumerable<GetMenuItemDto>> { }
}
