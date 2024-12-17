using Sales.Domain.Models.Enums;

namespace Sales.Application.DTOs.ProductDTO;

public record ProductAmountDto(
    ProductDTOOutput Product,
    decimal Amount
);