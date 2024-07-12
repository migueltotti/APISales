using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using ApiSales.Models.Enums;

namespace ApiSales.Models;

public class Product
{
    public Product()
    {
        Orders = new Collection<Order>();
    }
    
    [Key]
    public int ProductId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }
    public TypeValue TypeValue { get; set; }

    // Product n : 1 Category
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Product n : n Order
    public ICollection<Order>? Orders { get; set; }
}