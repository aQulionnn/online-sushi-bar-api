using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using MediatR;

namespace Application.Features.MenuItem.Commands
{
    public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, GetMenuItemDto>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IRedisService _redisService;

        public UpdateMenuItemCommandHandler(IMenuItemService menuItemService, IRedisService redisService)
        {
            _menuItemService = menuItemService;
            _redisService = redisService;
        }

        public async Task<GetMenuItemDto> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
        {
            await _redisService.DeleteDataAsync($"menuItem:{request.id}");
            var updatedMenuItem = await _menuItemService.UpdateAsync(request.id, request.UpdateMenuItemDto);
            await _redisService.SetDataAsync($"menuItem:{request.id}", updatedMenuItem);
            
            return updatedMenuItem;    
        }
    }

    public record UpdateMenuItemCommand(int id, UpdateMenuItemDto UpdateMenuItemDto) : IRequest<GetMenuItemDto> { }
}
