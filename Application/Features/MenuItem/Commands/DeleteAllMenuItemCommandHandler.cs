using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using MediatR;

namespace Application.Features.MenuItem.Commands
{
    public class DeleteAllMenuItemCommandHandler : IRequestHandler<DeleteAllMenuItemCommand, IEnumerable<GetMenuItemDto>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IRedisService _redisService;

        public DeleteAllMenuItemCommandHandler(IMenuItemService menuItemService, IRedisService redisService)
        {
            _menuItemService = menuItemService;
            _redisService = redisService;
        }

        public async Task<IEnumerable<GetMenuItemDto>> Handle(DeleteAllMenuItemCommand request, CancellationToken cancellationToken)
        {
            await _redisService.DeleteDataAsync("menuItems");
            return await _menuItemService.DeleteAllAsync();
        }
    }

    public record DeleteAllMenuItemCommand() : IRequest<IEnumerable<GetMenuItemDto>> { }
}
