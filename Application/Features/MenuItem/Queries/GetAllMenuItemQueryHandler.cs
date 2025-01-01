using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Parameters;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Queries
{
    public class GetAllMenuItemQueryHandler : IRequestHandler<GetAllMenuItemQuery, Result<IEnumerable<GetMenuItemDto>>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IRedisService _redisService;
        public GetAllMenuItemQueryHandler(IMenuItemService menuItemService, IRedisService redisService)
        {
            _menuItemService = menuItemService;
            _redisService = redisService;
        }

        public async Task<Result<IEnumerable<GetMenuItemDto>>> Handle(GetAllMenuItemQuery request, CancellationToken cancellationToken)
        {
            var cachedMenuItems = await _redisService.GetDataAsync<IEnumerable<GetMenuItemDto>>
                ($"menuItems:page:{request.Pagination.Page}:pageSize:{request.Pagination.PageSize}");

            if (cachedMenuItems != null)
                return Result<IEnumerable<GetMenuItemDto>>.Success(cachedMenuItems);

            var menuItems = await _menuItemService.GetAllAsync(request.Pagination);
            await _redisService.SetDataAsync($"menuItems:page:{request.Pagination.Page}:pageSize:{request.Pagination.PageSize}", menuItems);

            return Result<IEnumerable<GetMenuItemDto>>.Success(cachedMenuItems);
        }
    }

    public record GetAllMenuItemQuery(PaginationParameters Pagination) : IRequest<Result<IEnumerable<GetMenuItemDto>>> { }
}
