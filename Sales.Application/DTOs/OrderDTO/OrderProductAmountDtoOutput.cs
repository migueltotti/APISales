using Sales.Application.DTOs.ProductDTO;

namespace Sales.Application.DTOs.OrderDTO;

public record OrderProductAmountDtoOutput(
    OrderDTOOutput Order,
    List<ProductAmountDto> ProductAmount
);