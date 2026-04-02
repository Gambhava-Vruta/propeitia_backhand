using FluentValidation;
using Propertia.Models;

namespace Propertia.Validators
{
    public class PropertyInquiryValidator : AbstractValidator<PropertyInquiryDto>
    {
        public PropertyInquiryValidator()
        {
            // PropertyId must be greater than 0
            RuleFor(x => x.PropertyId)
                .GreaterThan(0)
                .WithMessage("PropertyId is required and must be greater than 0");

            // UserId must be greater than 0
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId is required and must be greater than 0");

            // Optional message length validation
            RuleFor(x => x.Message)
                .MaximumLength(500)
                .WithMessage("Message cannot exceed 500 characters");

            // InquiryDate validation (optional)
            RuleFor(x => x.InquiryDate)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("InquiryDate cannot be in the future");
        }
    }
}
