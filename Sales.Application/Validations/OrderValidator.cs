using FluentValidation;
using Sales.Application.DTOs.OrderDTO;
using Sales.Domain.Models;

namespace Sales.Application.Validations;

public class OrderValidator : AbstractValidator<OrderDTOInput>
{
    public OrderValidator()
    {
        RuleFor(o => o.TotalValue).NotEmpty()
            .PrecisionScale(10, 2, true);
        RuleFor(o => o.OrderDate).NotEmpty();
        RuleFor(o => o.UserId).NotEmpty();
        RuleFor(o => o.holder).MaximumLength(50);
        RuleFor(o => o.note).MaximumLength(300);
    }
}