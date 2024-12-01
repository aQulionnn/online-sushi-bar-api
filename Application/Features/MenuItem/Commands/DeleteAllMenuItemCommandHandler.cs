using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.MenuItem.Commands
{
    public class DeleteAllMenuItemCommandHandler : IRequestHandler<DeleteAllMenuItemCommand, IEnumerable<GetMenuItemDto>>
    {
        private readonly IMenuItemService _menuItemService;

        public DeleteAllMenuItemCommandHandler(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public async Task<IEnumerable<GetMenuItemDto>> Handle(DeleteAllMenuItemCommand request, CancellationToken cancellationToken)
        {
            return await _menuItemService.DeleteAllAsync();
        }
    }

    public record DeleteAllMenuItemCommand() : IRequest<IEnumerable<GetMenuItemDto>> { }
}
