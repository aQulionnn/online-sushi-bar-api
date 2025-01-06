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

        public GetSortedMenuItemsQueryHandler(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public async Task<Result<IEnumerable<GetMenuItemDto>>> Handle(GetSortedMenuItemsQuery request, CancellationToken cancellationToken)
        {
            var menuItems = await _menuItemService.GetAllWithSortingAsync(request.Sorting);
            return Result<IEnumerable<GetMenuItemDto>>.Success(menuItems);
        }
    }

    public record GetSortedMenuItemsQuery(SortingParameters Sorting) : IRequest<Result<IEnumerable<GetMenuItemDto>>>;
}
