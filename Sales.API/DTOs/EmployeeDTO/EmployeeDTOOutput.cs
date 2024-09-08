using Sales.API.Models.Enums;

namespace Sales.API.DTOs.EmployeeDTO;    

public record EmployeeDTOOutput(
    int EmployeeId,
    string Name,
    string Cpf,
    DateTime DateBirth,
    Permission Permission
);