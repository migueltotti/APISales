namespace Sales.Application.DTOs.OrderDTO;   

public record OrderDTOInput(
    int OrderId,
    decimal TotalValue,
    DateTime OrderDate,
    int UserId
);