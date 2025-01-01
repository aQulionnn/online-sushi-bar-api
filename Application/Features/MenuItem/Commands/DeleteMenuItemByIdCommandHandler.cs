using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Errors;
using DAL.SharedKernels;
using MediatR;

namespace Application.Features.MenuItem.Commands
{
    public class DeleteMenuItemByIdCommandHandler : IRequestHandler<DeleteMenuItemByIdCommand, Result<GetMenuItemDto>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IRedisService _redisService;

        public DeleteMenuItemByIdCommandHandler(IMenuItemService menuItemService, IRedisService redisService)
        {
            _menuItemService = menuItemService;
            _redisService = redisService;
        }

        public async Task<Result<GetMenuItemDto>> Handle(DeleteMenuItemByIdCommand request, CancellationToken cancellationToken)
        {
            var menuItem = await _menuItemService.DeleteByIdAsync(request.id);
            if (menuItem == null) 
                return Result<GetMenuItemDto>.Failure(MenuItemErrors.NotFound);

            await _redisService.DeleteDataAsync($"menuItem:{request.id}");

            return Result<GetMenuItemDto>.Success(menuItem);
        }
    }
    public record DeleteMenuItemByIdCommand(int id) : IRequest<Result<GetMenuItemDto>> { }
}
