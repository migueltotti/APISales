using FluentValidation;
using Sales.Application.DTOs.TokenDTO;

namespace Sales.Application.Validations;

public class RegisterValidator : AbstractValidator<RegisterModel>
{
    public RegisterValidator()
    {
        RuleFor(rm => rm.Username).NotEmpty()
            .Length(3, 80)
            .WithMessage("Username must be between 3 and 80 characters")
            .Must(CustomValidators.FirstLetterUpperCase)
            .WithMessage("First letter of username should consist of uppercase letter");
        RuleFor(l => l.Email).NotEmpty()
            .EmailAddress()
            .Length(10, 80);
        RuleFor(l => l.Password).NotEmpty()
            .Must(CustomValidators.PasswordFormat)
            .WithMessage("'Password' must contain at least 8 characters, one uppercase letter, one lowercase letter, one number and one special character");
    }
}