using Sales.Domain.Models.Enums;

namespace Sales.Application.DTOs.UserDTO;    

public record UserDTOOutput(
    int UserId,
    string Name,
    string Cpf,
    DateTime DateBirth,
    Role Role
);