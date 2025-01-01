using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Errors;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Queries
{
    public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, Result<GetMenuItemDto>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IRedisService _redisService;
        public GetMenuItemByIdQueryHandler(IMenuItemService menuItemService, IRedisService redisService)
        {
            _menuItemService = menuItemService;
            _redisService = redisService;
        }

        public async Task<Result<GetMenuItemDto>> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
        {
            var cachedMenuItem = await _redisService.GetDataAsync<GetMenuItemDto>($"menuItem:{request.id}");
            if (cachedMenuItem != null)
                return Result<GetMenuItemDto>.Success(cachedMenuItem);

            var menuItem = await _menuItemService.GetByIdAsync(request.id);
            if (menuItem == null) 
                return Result<GetMenuItemDto>.Failure(MenuItemErrors.NotFound);

            await _redisService.SetDataAsync($"menuItem:{request.id}", menuItem);

            return Result<GetMenuItemDto>.Success(menuItem);
        }
    }

    public record GetMenuItemByIdQuery(int id) : IRequest<Result<GetMenuItemDto>> { }
}
