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
);