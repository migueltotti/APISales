using ApiSales.Models.Enums;

namespace ApiSales.DTOs.EmployeeDTO;    

public record EmployeeDTOOutput(
    int EmployeeId,
    string Name,
    string Cpf,
    DateTime DateBirth,
    Permission Permission
);