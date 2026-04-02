using FluentValidation;
using Propertia.Models;

namespace Propertia.Validators
{
    public class PropertyAddressValidator : AbstractValidator<PropertyAddressdto>
    {
        public PropertyAddressValidator()
        {
            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required");

           
        }
    }
}
