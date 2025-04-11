using System.Text.RegularExpressions;

namespace Sales.Application.Validations;

public class CustomValidators
{
    public static bool FirstLetterUpperCase(string? name)
    {
        return String.IsNullOrWhiteSpace(name)
               || String.Equals(name[0].ToString(), name[0].ToString().ToUpper());
    }

    public static bool PasswordFormat(string? password)
    {
        return Regex.IsMatch(password, "^(?=.*\\d)(?=.*[!@#\\$%^&*])(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z])\\S.{8,30}$");
        // Must contain at least 8 characters, a-z, A-Z, 0-9 and no line breakers or whitespaces 
    }

    public static bool CpfFormat(string? cpf)
    {
        return Regex.IsMatch(cpf, @"^(\d{3}\.\d{3}\.\d{3}-\d{2}|\d{11})$");
    }
}