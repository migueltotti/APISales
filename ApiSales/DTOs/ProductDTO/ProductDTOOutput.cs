using ApiSales.Models.Enums;

namespace ApiSales.DTOs.ProductDTO;

public record ProductDTOOutput(
    int ProductId,
    string Name,
    string Description,
    decimal Value,
    TypeValue TypeValue,
    int CategoryId
    );