using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Queries;

public class GetByWeightedSearchTermWithRankQueryHandler 
    : IRequestHandler<GetByWeightedSearchTermWithRankQuery, Result<IEnumerable<GetMenuItemDto>>>
{
    private readonly IMenuItemService _menuItemService;

    public GetByWeightedSearchTermWithRankQueryHandler(IMenuItemService menuItemService)
    {
        _menuItemService = menuItemService;
    }
    
    public async Task<Result<IEnumerable<GetMenuItemDto>>> Handle(GetByWeightedSearchTermWithRankQuery request, CancellationToken cancellationToken)
    {
        var result = await _menuItemService.GetByWeightedSearchTermWithRank(request.SearchTerm);
        return Result<IEnumerable<GetMenuItemDto>>.Success(result);
    }
}

public record GetByWeightedSearchTermWithRankQuery(string SearchTerm) 
    : IRequest<Result<IEnumerable<GetMenuItemDto>>> { }