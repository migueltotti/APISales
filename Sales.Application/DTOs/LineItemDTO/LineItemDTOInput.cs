using Sales.Application.DTOs.ProductDTO;

namespace Sales.Application.DTOs.LineItemDTO;

public record LineItemDTOInput(
    int ProductId,
    decimal Amount,
    decimal Price
);