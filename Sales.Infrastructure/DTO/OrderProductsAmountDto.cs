using Sales.Domain.Models.Enums;

namespace Sales.Infrastructure.DTO;

public record OrderProductsAmountDto(
    int? OrderId,
    decimal? TotalValue,
    DateTime? OrderDate,
    Status? OrderStatus,
    int? UserId,
    int? ProductId,
    string? Name,
    string? Description,
    decimal? Value,
    TypeValue? TypeValue,
    string? ImageUrl,
    int? StockQuantity,
    int? CategoryId,
    decimal? ProductAmount
);