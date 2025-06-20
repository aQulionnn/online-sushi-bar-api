using Application.Delegates;
using Application.Interfaces;
using Application.Validators.MenuItem;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Enums;
using DAL.Errors;
using DAL.Parameters;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Queries
{
    public class GetSortedMenuItemsQueryHandler : IRequestHandler<GetSortedMenuItemsQuery, Result<IEnumerable<GetMenuItemDto>>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly CacheServiceResolver  _cacheServiceResolver;
        private readonly SortingMenuItemsValidator _validator;

        public GetSortedMenuItemsQueryHandler
            (
                IMenuItemService menuItemService, 
                SortingMenuItemsValidator validator, 
                CacheServiceResolver cacheServiceResolver)
        {
            _menuItemService = menuItemService;
            _validator = validator;
            _cacheServiceResolver = cacheServiceResolver;
        }

        public async Task<Result<IEnumerable<GetMenuItemDto>>> Handle(GetSortedMenuItemsQuery request, CancellationToken cancellationToken)
        {
            var redisCacheService = _cacheServiceResolver(CachingType.Redis);
            
            var validation = await _validator.ValidateAsync(request.Sorting);
            if (!validation.IsValid)
            {
                var problemDetails = validation.Errors.Select(e => new { Message = e.ErrorMessage }).ToList();
                return Result<IEnumerable<GetMenuItemDto>>.Failure(MenuItemErrors.ValidationError(problemDetails));
            }

            var cachedMenuItems = await redisCacheService.GetDataAsync<IEnumerable<GetMenuItemDto>>($"menuItems:sorted:{request.Sorting.GetHashCode()}");
            if (cachedMenuItems != null)
                return Result<IEnumerable<GetMenuItemDto>>.Success(cachedMenuItems);

            var menuItems = await _menuItemService.GetAllWithSortingAsync(request.Sorting);
            await redisCacheService.SetDataAsync($"menuItems:sorted:{request.Sorting.GetHashCode()}", menuItems);

            return Result<IEnumerable<GetMenuItemDto>>.Success(menuItems);
        }
    }

    public record GetSortedMenuItemsQuery(SortingParameters Sorting) : IRequest<Result<IEnumerable<GetMenuItemDto>>>;
}
