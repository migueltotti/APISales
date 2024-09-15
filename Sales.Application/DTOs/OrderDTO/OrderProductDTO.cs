using Sales.Domain.Models;

namespace Sales.Application.DTOs.OrderDTO;  

public record OrderProductDTO(
    int OrderId,
    decimal TotalValue,
    DateTime OrderDate,
    int UserId,
    IEnumerable<Product> Products
);