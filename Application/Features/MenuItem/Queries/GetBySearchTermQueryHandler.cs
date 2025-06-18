using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Queries;

public class GetBySearchTermQueryHandler : IRequestHandler<GetBySearchTermQuery, Result<IEnumerable<GetMenuItemDto>>>
{
    private readonly IMenuItemService _menuItemService;

    public GetBySearchTermQueryHandler(IMenuItemService menuItemService)
    {
        _menuItemService = menuItemService;
    }
    
    public async Task<Result<IEnumerable<GetMenuItemDto>>> Handle(GetBySearchTermQuery request, CancellationToken cancellationToken)
    {
        var result = await _menuItemService.GetBySearchTerm(request.SearchTerm);
        return Result<IEnumerable<GetMenuItemDto>>.Success(result);
    }
}

public record GetBySearchTermQuery(string SearchTerm) 
    : IRequest<Result<IEnumerable<GetMenuItemDto>>> { }