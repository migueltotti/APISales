using Sales.API.Models.Enums;

namespace Sales.API.DTOs.ProductDTO; 

public record ProductDTOInput(
    int ProductId,
    string Name,
    string Description,
    decimal Value,
    TypeValue TypeValue,
    int CategoryId
    );