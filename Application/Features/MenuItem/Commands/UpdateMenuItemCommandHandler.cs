using Application.Delegates;
using Application.Interfaces;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Enums;
using DAL.Errors;
using DAL.SharedKernels;
using FluentValidation;
using MediatR;

namespace Application.Features.MenuItem.Commands
{
    public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, Result<GetMenuItemDto>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly CacheServiceResolver  _cacheServiceResolver;
        private readonly IValidator<UpdateMenuItemDto> _validator;

        public UpdateMenuItemCommandHandler
            (
                IMenuItemService menuItemService, 
                 
                IValidator<UpdateMenuItemDto> validator, CacheServiceResolver cacheServiceResolver)
        {
            _menuItemService = menuItemService;
            
            _validator = validator;
            _cacheServiceResolver = cacheServiceResolver;
        }

        public async Task<Result<GetMenuItemDto>> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
        {
            var redisCacheService = _cacheServiceResolver(CachingType.Redis);
            
            var updatedMenuItem = await _menuItemService.UpdateAsync(request.id, request.UpdateMenuItemDto);
            if (updatedMenuItem == null) 
                return Result<GetMenuItemDto>.Failure(MenuItemErrors.NotFound);

            var validation = await _validator.ValidateAsync(request.UpdateMenuItemDto);
            if (!validation.IsValid)
            {
                var problemDetails = validation.Errors.Select(e => new { Message = e.ErrorMessage }).ToList();
                return Result<GetMenuItemDto>.Failure(MenuItemErrors.ValidationError(problemDetails));
            }

            await redisCacheService.DeleteDataAsync($"menuItem:{request.id}");        
            await redisCacheService.SetDataAsync($"menuItem:{request.id}", updatedMenuItem);
            
            return Result<GetMenuItemDto>.Success(updatedMenuItem);    
        }
    }

    public record UpdateMenuItemCommand(int id, UpdateMenuItemDto UpdateMenuItemDto) : IRequest<Result<GetMenuItemDto>> { }
}
