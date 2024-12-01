using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using MediatR;

namespace Application.Features.MenuItem.Commands
{
    public class DeleteMenuItemByIdCommandHandler : IRequestHandler<DeleteMenuItemByIdCommand, GetMenuItemDto>
    {
        private readonly IMenuItemService _menuItemService;

        public DeleteMenuItemByIdCommandHandler(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public async Task<GetMenuItemDto> Handle(DeleteMenuItemByIdCommand request, CancellationToken cancellationToken)
        {
            return await _menuItemService.DeleteByIdAsync(request.id);
        }
    }
    public record DeleteMenuItemByIdCommand(int id) : IRequest<GetMenuItemDto> { }
}
