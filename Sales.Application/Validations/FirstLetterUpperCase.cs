using System.ComponentModel.DataAnnotations;

namespace Sales.Application.Validations;

public class FirstLetterUpperCase : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var firstLetter = value.ToString()[0].ToString();

        if (!String.Equals(firstLetter, firstLetter.ToUpper()))
        {
            return new ValidationResult("Name first letter must be upper case");
        }
        
        return ValidationResult.Success;
    }
}