namespace Sales.Application.DTOs.OrderDTO;   

public record OrderDTOInput(
    int OrderId,
    decimal TotalValue,
    DateTime Date,
    int EmployeeId
);