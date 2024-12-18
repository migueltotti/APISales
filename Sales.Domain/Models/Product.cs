using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Sales.Domain.Models.Enums;

namespace Sales.Domain.Models;

public sealed class Product
{
    public int ProductId { get; private set; }
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Value { get; private set; }
    public TypeValue TypeValue { get; private set; }
    public string? ImageUrl { get; private set; }
    public int StockQuantity { get; private set; }
    
    // Product n : 1 Category
    public int CategoryId { get; private set; }
    public Category? Category { get; private set; }
    
    // Product n : n ShoppingCart
    public ICollection<ShoppingCart>? ShoppingCart { get; private set; }

    private Product()
    {
    }

    public Product(int productId, string? name, string? description, decimal value, TypeValue typeValue, string? imageUrl, int stockQuantity, int categoryId)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Value = value;
        TypeValue = typeValue;
        ImageUrl = imageUrl;
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
    }

    public Product(string? name, string? description, decimal value, TypeValue typeValue, string? imageUrl, int stockQuantity, int categoryId)
    {
        Name = name;
        Description = description;
        Value = value;
        TypeValue = typeValue;
        ImageUrl = imageUrl;
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
    }

    public void DecreaseStockQuantity()
    {
        StockQuantity--;
    }
}