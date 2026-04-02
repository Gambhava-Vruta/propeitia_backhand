using FluentValidation;
using Propertia.Models;

namespace Propertia.Validators
{
    public class CategoryValidator : AbstractValidator<Categorydto>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.CategoryName).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Category name is required")
                .MinimumLength(2).WithMessage("Category name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");

        }
    }
}
