using Sales.Application.DTOs.ProductDTO;

namespace Sales.Application.DTOs.ShoppingCartDTO;

public record ShoppingCartDTOOutput(
    int ShoppingCartId,
    double TotalValue,
    int ProductsCount,
    int UserId,
    List<ProductCheckedDTO> Products
);