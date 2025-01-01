using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Errors;
using DAL.SharedKernels;
using FluentValidation;
using MediatR;

namespace Application.Features.MenuItem.Commands
{
    public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Result<GetMenuItemDto>>
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IValidator<CreateMenuItemDto> _validator;

        public CreateMenuItemCommandHandler(IMenuItemService menuItemService, IValidator<CreateMenuItemDto> validator)
        {
            _menuItemService = menuItemService;
            _validator = validator;
        }

        public async Task<Result<GetMenuItemDto>> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.CreateMenuItemDto);
            if (!validation.IsValid)
            {
                var problemDetails = validation.Errors.Select(e => new { Message = e.ErrorMessage }).ToList();
                return Result<GetMenuItemDto>.Failure(MenuItemErrors.ValidationError(problemDetails));
            }

            var menuItem = await _menuItemService.CreateAsync(request.CreateMenuItemDto);
            return Result<GetMenuItemDto>.Success(menuItem);
        }
    }

    public record CreateMenuItemCommand(CreateMenuItemDto CreateMenuItemDto) : IRequest<Result<GetMenuItemDto>> { }
}
