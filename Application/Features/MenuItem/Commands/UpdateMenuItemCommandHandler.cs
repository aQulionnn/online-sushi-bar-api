using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using MediatR;

namespace Application.Features.MenuItem.Commands
{
    public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, GetMenuItemDto>
    {
        private readonly IMenuItemService _menuItemService;

        public UpdateMenuItemCommandHandler(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public async Task<GetMenuItemDto> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
        {
            return await _menuItemService.UpdateAsync(request.id, request.UpdateMenuItemDto);
        }
    }

    public record UpdateMenuItemCommand(int id, UpdateMenuItemDto UpdateMenuItemDto) : IRequest<GetMenuItemDto> { }
}
