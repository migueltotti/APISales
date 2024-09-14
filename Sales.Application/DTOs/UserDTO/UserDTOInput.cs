using Sales.Domain.Models.Enums;

namespace Sales.Application.DTOs.UserDTO;    

public record UserDTOInput(
    int UserId,
    string Name,
    string Email,
    string Password,
    string Cpf,
    DateTime DateBirth,
    Role Role
);