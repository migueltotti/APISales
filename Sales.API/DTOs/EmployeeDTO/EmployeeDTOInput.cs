using Sales.API.Models.Enums;

namespace Sales.API.DTOs.EmployeeDTO;    

public record EmployeeDTOInput(
    int EmployeeId,
    string Name,
    string Cpf,
    DateTime DateBirth,
    Permission Permission
);