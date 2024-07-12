using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using ApiSales.Validations;

namespace ApiSales.Models;

public class Category
{
    public Category()
    {
        Products = new Collection<Product>();
    }
    
    [Key]
    public int CategoryId { get; set; }
    
    [Required(ErrorMessage = "Category name is required!")]
    [StringLength(50, ErrorMessage = "Name must be less than {1} and more than {3}",MinimumLength = 5)]
    [FirstLetterUpperCase]
    public string? Name { get; set; }
    
    // Category 1 : n Product
    public ICollection<Product>? Products { get; set; }
}