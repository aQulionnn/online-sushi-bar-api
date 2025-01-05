using DAL.Parameters;
using FluentValidation;

namespace Application.Validators.MenuItem
{
    public class SortingMenuItemsValidator : AbstractValidator<SortingParameters>
    {
        public SortingMenuItemsValidator()
        {
            RuleFor(x => x)
                .Custom((sortingParameters, context) =>
                {
                    if (sortingParameters.ByPrice && sortingParameters.ByName)
                        context.AddFailure("You can only sort by one criteria: ByName or ByPrice.");
                });
        }
    }
}
