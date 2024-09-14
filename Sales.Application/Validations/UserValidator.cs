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
            .WithMessage("Name must be between 3 and 80 characters")
            .Must(CustomValidators.FirstLetterUpperCase)
            .WithMessage("First letter of name should consist of uppercase letter");
        RuleFor(u => u.Email).NotEmpty()
            .EmailAddress()
            .Length(10, 80);
        RuleFor(u => u.Password).NotEmpty()
            .Must(CustomValidators.PasswordFormat)
            .WithMessage("'Password' must contain at least 8 characters, one uppercase letter, one lowercase letter, one number and one special character");
        RuleFor(u => u.Cpf).NotEmpty()
            .Must(CustomValidators.CpfFormat)
            .WithMessage("'CPF' must be in format 'xxx.xxx.xxx-xx' or 'xxxxxxxxxxx'");
        RuleFor(u => u.Role).IsInEnum();
    }
}