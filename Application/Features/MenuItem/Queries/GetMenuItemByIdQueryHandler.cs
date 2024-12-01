using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.MenuItem.Queries
{
    public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, GetMenuItemDto>
    {
        private readonly IMenuItemService _menuItemService;
        public GetMenuItemByIdQueryHandler(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public async Task<GetMenuItemDto> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
        {
            return await _menuItemService.GetByIdAsync(request.id);
        }
    }

    public record GetMenuItemByIdQuery(int id) : IRequest<GetMenuItemDto> { }
}
