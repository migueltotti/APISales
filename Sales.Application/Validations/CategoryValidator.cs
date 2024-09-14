using FluentValidation;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Domain.Models;

namespace Sales.Application.Validations;

public class CategoryValidator : AbstractValidator<CategoryDTOInput>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Name).NotEmpty()
            .Length(5, 80)
            .Must(CustomValidators.FirstLetterUpperCase);
        RuleFor(c => c.ImageUrl).Length(1, 250);
    }
}