using Sales.Application.DTOs.ProductDTO;
using Sales.Domain.Models;
using Sales.Domain.Models.Enums;

namespace Sales.Application.DTOs.OrderDTO;  

public record OrderProductsDTO(
    int OrderId,
    decimal TotalValue,
    DateTime OrderDate,
    Status OrderStatus,
    int UserId,
    IEnumerable<ProductDTOOutput> Products
);