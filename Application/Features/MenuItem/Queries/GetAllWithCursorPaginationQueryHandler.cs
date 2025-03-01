using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Parameters;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Queries;

public class GetAllWithCursorPaginationQueryHandler : IRequestHandler<GetAllWithCursorPaginationQuery, Result<CursorPagedResult<GetMenuItemDto>>>
{
    private readonly IMenuItemService _menuItemService;

    public GetAllWithCursorPaginationQueryHandler(IMenuItemService menuItemService)
    {
        _menuItemService = menuItemService;
    }

    public async Task<Result<CursorPagedResult<GetMenuItemDto>>> Handle(GetAllWithCursorPaginationQuery request, CancellationToken cancellationToken)
    {
        var result = await _menuItemService.GetAllWithCursorPaginationAsync(request.CursorPaginationParameters);
        return Result<CursorPagedResult<GetMenuItemDto>>.Success(result);
    }
}

public record GetAllWithCursorPaginationQuery(CursorPaginationParameters CursorPaginationParameters): 
    IRequest<Result<CursorPagedResult<GetMenuItemDto>>> { }