using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Commands
{
    public class DeleteAllMenuItemCommandHandler : IRequestHandler<DeleteAllMenuItemCommand, Result<IEnumerable<GetMenuItemDto>>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IRedisService _redisService;

        public DeleteAllMenuItemCommandHandler(IMenuItemService menuItemService, IRedisService redisService)
        {
            _menuItemService = menuItemService;
            _redisService = redisService;
        }

        public async Task<Result<IEnumerable<GetMenuItemDto>>> Handle(DeleteAllMenuItemCommand request, CancellationToken cancellationToken)
        {
            var menuItems = await _menuItemService.DeleteAllAsync();
            await _redisService.DeleteDataAsync("menuItems");
            return Result<IEnumerable<GetMenuItemDto>>.Success(menuItems);
        }
    }

    public record DeleteAllMenuItemCommand() : IRequest<Result<IEnumerable<GetMenuItemDto>>> { }
}
