using BLL.Dtos.MenuItem;
using FluentValidation;

namespace Application.Validators.MenuItem
{
    public class CreateMenuItemDtoValidator : AbstractValidator<CreateMenuItemDto>
    {
        public CreateMenuItemDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(25).WithMessage("Name cannot exceed 25 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price is required.")
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Category.ToString())
                .Must(category => category == "Sushi" || category == "Drink")
                .WithMessage("Category must be either 'Sushi' or 'Drink'.");
        }
    }
}
