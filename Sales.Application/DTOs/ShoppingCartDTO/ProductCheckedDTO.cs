using Sales.Application.DTOs.ProductDTO;
using Sales.Domain.Models;

namespace Sales.Application.DTOs.ShoppingCartDTO;

public record ProductCheckedDTO( 
    ProductDTOOutput Product,    
    bool? Checked,
    decimal? Amount
);