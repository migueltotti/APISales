namespace Sales.API.DTOs.OrderDTO;   

public record OrderDTOInput(
    int OrderId,
    decimal TotalValue,
    DateTime Date,
    int EmployeeId
);