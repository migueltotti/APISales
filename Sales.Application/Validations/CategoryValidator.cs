using System.Drawing;
using FluentValidation;
using Sales.Domain.Models;

namespace Sales.Application.Validations;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
    }
}