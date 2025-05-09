using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Sales.Domain.Models.Enums;

namespace Sales.Application.DTOs.UserDTO;

public record UserDTOInput(
    int UserId,
    string Name,
    string Email,
    string Password,
    string Cpf,
    decimal Points,
    DateTime DateBirth,
    int AffiliateId,
    Role Role
)
{
    public string GenerateUserName()
    {
        return Name.Replace(" ", "")
               + "-"
               + Email[1]
               + Email[2]
               + Email[0]
               + Email[8]
               + Email[2]
               + Email[2];
    }
}