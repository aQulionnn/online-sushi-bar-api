using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using MediatR;

namespace Application.Features.MenuItem.Commands
{
    public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, GetMenuItemDto>
    {
        private readonly IMenuItemService _menuItemService;
        public CreateMenuItemCommandHandler(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public async Task<GetMenuItemDto> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
        {
            return await _menuItemService.CreateAsync(request.CreateMenuItemDto);
        }
    }

    public record CreateMenuItemCommand(CreateMenuItemDto CreateMenuItemDto) : IRequest<GetMenuItemDto> { }
}
