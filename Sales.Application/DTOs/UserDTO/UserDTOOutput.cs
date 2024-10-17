using Sales.Domain.Models.Enums;

namespace Sales.Application.DTOs.UserDTO;    

public record UserDTOOutput(
    int UserId,
    string Name,
    string Email,
    string Cpf,
    decimal Points,
    DateTime DateBirth,
    int AffiliateId,
    Role Role
);