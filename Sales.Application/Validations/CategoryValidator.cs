using FluentValidation;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Domain.Models;

namespace Sales.Application.Validations;

public class CategoryValidator : AbstractValidator<CategoryDTOInput>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Name).NotEmpty()
            .Length(3, 80)
            .Must(CustomValidators.FirstLetterUpperCase)
            .WithMessage("First letter of 'Name' should consist of uppercase letter.");
        RuleFor(c => c.ImageUrl).Length(1, 250);
    }
}