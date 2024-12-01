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
    public class GetAllMenuItemQueryHandler : IRequestHandler<GetAllMenuItemQuery, IEnumerable<GetMenuItemDto>>
    {
        private readonly IMenuItemService _menuItemService;
        public GetAllMenuItemQueryHandler(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public async Task<IEnumerable<GetMenuItemDto>> Handle(GetAllMenuItemQuery request, CancellationToken cancellationToken)
        {
            return await _menuItemService.GetAllAsync();
        }
    }

    public record GetAllMenuItemQuery() : IRequest<IEnumerable<GetMenuItemDto>> { }
}
