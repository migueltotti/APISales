using FluentValidation;
using Sales.Application.DTOs.AffiliateDTO;
using Sales.Domain.Models;

namespace Sales.Application.Validations;

public class AffiliateValidator : AbstractValidator<AffiliateDTOInput>
{
    public AffiliateValidator()
    {
        RuleFor(a => a.Name).NotEmpty()
            .Length(1, 50)
            .Must(CustomValidators.FirstLetterUpperCase)
            .WithMessage("First letter of 'Name' should consist of uppercase letter.");
        RuleFor(a => a.Discount).NotEmpty()
            .InclusiveBetween(0.00m, 80.00m)
            .PrecisionScale(4, 2, true)
            .WithMessage("Discount must be between 0.00% and 80.00%");
    }
}