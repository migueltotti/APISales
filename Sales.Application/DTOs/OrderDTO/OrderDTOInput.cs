using Sales.Domain.Models.Enums;

namespace Sales.Application.DTOs.OrderDTO;   

public record OrderDTOInput(
    int OrderId,
    decimal TotalValue,
    DateTime OrderDate,
    Status OrderStatus,
    string? Holder,
    string? Note,
    int UserId
);