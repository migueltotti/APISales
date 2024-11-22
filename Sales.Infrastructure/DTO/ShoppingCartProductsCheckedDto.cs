using Sales.Domain.Models;
using Sales.Domain.Models.Enums;

namespace Sales.Infrastructure.DTO;

public class ShoppingCartProductsCheckedDto
{
    public int? ShoppingCartId { get; set; }
    public double? TotalValue { get; set; }
    public int? ProductsCount { get; set; }
    public int? UserId { get; set; }
    public int? ProductId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Value { get; set; }
    public TypeValue? TypeValue { get; set; }
    public string? ImageUrl { get; set; }
    public int? StockQuantity { get; set; }
    public int? CategoryId { get; set; }
    public bool? Checked { get; set; }
    public decimal? Amount { get; set; }
}   