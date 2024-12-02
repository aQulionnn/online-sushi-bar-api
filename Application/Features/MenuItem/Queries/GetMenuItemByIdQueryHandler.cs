using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using MediatR;

namespace Application.Features.MenuItem.Queries
{
    public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, GetMenuItemDto>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IRedisService _redisService;
        public GetMenuItemByIdQueryHandler(IMenuItemService menuItemService, IRedisService redisService)
        {
            _menuItemService = menuItemService;
            _redisService = redisService;
        }

        public async Task<GetMenuItemDto> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
        {
            var cachedMenuItem = await _redisService.GetDataAsync<GetMenuItemDto>($"menuItem:{request.id}");
            if (cachedMenuItem != null)
                return cachedMenuItem;

            var menuItem = await _menuItemService.GetByIdAsync(request.id);
            await _redisService.SetDataAsync($"menuItem:{request.id}", menuItem);

            return menuItem;
        }
    }

    public record GetMenuItemByIdQuery(int id) : IRequest<GetMenuItemDto> { }
}
