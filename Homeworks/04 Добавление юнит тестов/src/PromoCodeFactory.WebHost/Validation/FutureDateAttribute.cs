using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.WebHost.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTimeOffset date)
            return ValidationResult.Success;

        if (date <= DateTimeOffset.UtcNow)
            return new ValidationResult(ErrorMessage ?? "Date must be in the future.");

        return ValidationResult.Success;
    }
}
