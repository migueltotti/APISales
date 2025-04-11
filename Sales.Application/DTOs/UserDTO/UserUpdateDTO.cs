using Sales.Domain.Models.Enums;

namespace Sales.Application.DTOs.UserDTO;

public record UserUpdateDTO(
    int UserId,
    string Name,
    string Email,
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