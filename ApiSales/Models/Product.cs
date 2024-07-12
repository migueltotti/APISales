using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using ApiSales.Models.Enums;
using ApiSales.Validations;

namespace ApiSales.Models;

public class Product
{

    public Product()
    {
        Orders = new Collection<Order>();
    }
    
    [Key]
    public int ProductId { get; set; }
    
    [Required(ErrorMessage = "Product name is required!")]
    [StringLength(50, ErrorMessage = "Name must be less than {1} and more than {3}",MinimumLength = 5)]
    [FirstLetterUpperCase]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Description is required!")]
    [StringLength(300, ErrorMessage = "Name must be less than {1} and more than {3}",MinimumLength = 10)]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Value is required!")]
    public decimal Value { get; set; }
    
    [Required(ErrorMessage = "TypeValue is required!")]
    public TypeValue TypeValue { get; set; }

    // Product n : 1 Category
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Product n : n Order
    public ICollection<Order>? Orders { get; set; }
}