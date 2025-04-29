using Sales.Application.DTOs.ProductDTO;

namespace Sales.Application.DTOs.LineItemDTO;

public record LineItemDTOOutput(
    int LineItemId,
    int OrderId,
    int ProductId,
    decimal Amount,
    decimal Price,
    ProductDTOOutput Product
);