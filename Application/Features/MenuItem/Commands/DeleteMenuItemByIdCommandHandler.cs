using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using MediatR;

namespace Application.Features.MenuItem.Commands
{
    public class DeleteMenuItemByIdCommandHandler : IRequestHandler<DeleteMenuItemByIdCommand, GetMenuItemDto>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IRedisService _redisService;

        public DeleteMenuItemByIdCommandHandler(IMenuItemService menuItemService, IRedisService redisService)
        {
            _menuItemService = menuItemService;
            _redisService = redisService;
        }

        public async Task<GetMenuItemDto> Handle(DeleteMenuItemByIdCommand request, CancellationToken cancellationToken)
        {
            await _redisService.DeleteDataAsync($"menuItem:{request.id}");
            return await _menuItemService.DeleteByIdAsync(request.id);
        }
    }
    public record DeleteMenuItemByIdCommand(int id) : IRequest<GetMenuItemDto> { }
}
