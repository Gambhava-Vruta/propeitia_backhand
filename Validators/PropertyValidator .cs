using FluentValidation;
using Propertia.Models;

namespace Propertia.Validators
{
    public class PropertyCreateValidator : AbstractValidator<PropertyCreateDto>
    {
        public PropertyCreateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.AreaSqft)
                .GreaterThan(0).WithMessage("AreaSqft must be greater than 0");

            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(x => x == "ongoing" || x == "completed")
                .WithMessage("Status must be ongoing or completed");

            RuleFor(x => x.RequireType)
                .NotEmpty()
                .Must(x => x == "rent" || x == "sale" || x == "any")
                .WithMessage("RequireType must be rent, sale, or any");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId is required");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId is required");

            RuleFor(x => x.Address)
                .NotNull().WithMessage("Address is required");

            RuleFor(x => x.BHKType)
                .NotEmpty().WithMessage("BHKType is required");

            RuleFor(x => x.TotalWashrooms)
                .GreaterThanOrEqualTo(0)
                .WithMessage("TotalWashrooms cannot be negative");

            RuleFor(x => x.SalePrice)
                .NotNull()
                .GreaterThan(0)
                .When(x => x.RequireType == "sale" || x.RequireType == "any")
                .WithMessage("SalePrice is required for sale properties");

            RuleFor(x => x.RentPrice)
                .NotNull()
                .GreaterThan(0)
                .When(x => x.RequireType == "rent" || x.RequireType == "any")
                .WithMessage("RentPrice is required for rent properties");

            RuleFor(x => x.Images)
                .Must(x => x == null || x.Count <= 10)
                .WithMessage("Maximum 10 images are allowed");

            RuleForEach(x => x.AmenityIds)
                .GreaterThanOrEqualTo(0)
                .When(x => x.AmenityIds != null);
        }
    }
}
