namespace Sales.Application.DTOs.OrderDTO;

public record OrderDTOOutput(
    int OrderId,
    decimal TotalValue,
    DateTime Date,
    int EmployeeId
);