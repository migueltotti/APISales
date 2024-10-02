using Sales.Domain.Models.Enums;

namespace Sales.Application.DTOs.ProductDTO;

public record ProductDTOOutput(
    int ProductId,
    string Name,
    string Description,
    decimal Value,
    TypeValue TypeValue,
    int StockQuantity,
    string ImageUrl,
    int CategoryId
    );