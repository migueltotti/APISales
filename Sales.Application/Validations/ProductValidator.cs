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
            .Must(CustomValidators.FirstLetterUpperCase).WithMessage("First letter of name should consist of uppercase letter");
        RuleFor(p => p.Description).NotEmpty()
            .Length(1, 175);
        RuleFor(p => p.Value).NotEmpty()
            .PrecisionScale(10, 2, true);
        RuleFor(p => p.TypeValue).IsInEnum()
            .NotEmpty();
        RuleFor(p => p.ImageUrl).Length(1, 250);
        RuleFor(p => p.StockQuantity).InclusiveBetween(1, 80);
        RuleFor(p => p.CategoryId).NotEmpty();
    }
    
}