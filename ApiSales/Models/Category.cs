using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ApiSales.Models;

public class Category
{
    public Category()
    {
        Products = new Collection<Product>();
    }
    
    [Key]
    public int CategoryId { get; set; }
    public string? Name { get; set; }
    
    // Category 1 : n Product
    public ICollection<Product>? Products { get; set; }
}