using FluentValidation;
using Propertia.Models;

namespace Propertia.Validators
{
    public class UserValidator : AbstractValidator<Userdto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(3);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.Phone)
                .Matches(@"^[6-9]\d{9}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.UserType)
                .NotEmpty()
                .Must(x => x == "buyer" || x == "seller" || x == "admin")
                .Unless(x=>string.IsNullOrEmpty(x.UserType))
                .WithMessage("UserType must be buyer or seller");
        }
    }
}
