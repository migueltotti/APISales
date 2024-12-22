using Sales.Domain.Models;
using Sales.Domain.Models.Enums;

namespace Sales.Application.DTOs.OrderDTO;

public record OrderDTOOutput(
    int OrderId,
    decimal TotalValue,
    DateTime OrderDate,
    Status OrderStatus,
    int UserId,
    string? Holder,
    string? Note,
    List<LineItem>? LineItems
);