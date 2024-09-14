using FluentValidation;
using Sales.Application.DTOs.ProductDTO;
using Sales.Domain.Models;

namespace Sales.Application.Validations;

public class ProductValidator : AbstractValidator<ProductDTOInput>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name).NotEmpty()
            .Length(1, 80)
            .Must(CustomValidators.FirstLetterUpperCase)
            .WithMessage("First letter of name should consist of uppercase letter");
        RuleFor(p => p.Description).NotEmpty()
            .Length(10, 175)
            .WithMessage("'Description' must be between 10 and 175 characters.");
        RuleFor(p => p.Value).NotEmpty()
            .PrecisionScale(10, 2, true);
        RuleFor(p => p.TypeValue).IsInEnum()
            .NotEmpty();
        RuleFor(p => p.ImageUrl).Length(1, 250);
        RuleFor(p => p.StockQuantity).InclusiveBetween(1, 80);
        RuleFor(p => p.CategoryId).Must(categoryId => categoryId >= 0);
    }
    
}