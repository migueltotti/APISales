namespace Sales.Application.DTOs.OrderDTO;

public record OrderDTOOutput(
    int OrderId,
    decimal TotalValue,
    DateTime OrderDate,
    int UserId
);