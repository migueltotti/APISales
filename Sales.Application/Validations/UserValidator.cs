using FluentValidation;
using Sales.Application.DTOs.UserDTO;
using Sales.Domain.Models;

namespace Sales.Application.Validations;

public class UserValidator : AbstractValidator<UserDTOInput>
{
    public UserValidator()
    {
        RuleFor(u => u.Name).NotEmpty()
            .Length(3, 80)
            .Must(CustomValidators.FirstLetterUpperCase);
        RuleFor(u => u.Email).NotEmpty()
            .EmailAddress()
            .Length(10, 80);
        RuleFor(u => u.Password).NotEmpty()
            .Must(CustomValidators.PasswordFormat)
            .Length(8, 30);
        RuleFor(u => u.Cpf).NotEmpty()
            .Must(CustomValidators.CpfFormat);
        RuleFor(u => u.Role).NotEmpty().IsInEnum();
    }
}