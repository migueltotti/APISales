using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ApiSales.Models;

public class Category
{
    
    [Key]
    public int CategoryId { get; set; }
    public string? Name { get; set; }
    
    // Category 1 : n Product
    public ICollection<Product>? Products = [];
}