using FluentValidation;
using Sales.Application.DTOs.TokenDTO;

namespace Sales.Application.Validations;

public class LoginValidator : AbstractValidator<LoginModel>
{
    public LoginValidator()
    {
        //RuleFor(l => l.Username).NotEmpty()
        //    .WithMessage("Username is required");
        RuleFor(l => l.Email).NotEmpty()
            .EmailAddress()
            .Length(10, 80);
        RuleFor(l => l.Password).NotEmpty()
            .Must(CustomValidators.PasswordFormat)
            .WithMessage("'Password' must contain at least 8 characters, one uppercase letter, one lowercase letter, one number and one special character");
    }
}