using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Queries;

public class GetBySearchTermWithRankQueryHandler : IRequestHandler<GetBySearchTermWithRankQuery, Result<IEnumerable<GetMenuItemDto>>>
{
    private readonly IMenuItemService _menuItemService;

    public GetBySearchTermWithRankQueryHandler(IMenuItemService menuItemService)
    {
        _menuItemService = menuItemService;
    }
    
    public async Task<Result<IEnumerable<GetMenuItemDto>>> Handle(GetBySearchTermWithRankQuery request, CancellationToken cancellationToken)
    {
        var result = await _menuItemService.GetBySearchTermWithRank(request.SearchTerm);
        return Result<IEnumerable<GetMenuItemDto>>.Success(result);         
    }
}

public record GetBySearchTermWithRankQuery(string SearchTerm) 
    : IRequest<Result<IEnumerable<GetMenuItemDto>>> { }