using ApiSales.Models.Enums;

namespace ApiSales.DTOs.EmployeeDTO;    

public record EmployeeDTOInput(
    int EmployeeId,
    string Name,
    string Cpf,
    DateTime DateBirth,
    Permission Permission
);