using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ApiSales.Models;

public class Order
{
    
    [Key]
    public int OrderId { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime Date { get; set; }

    // Order n : 1 Employee
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    
    // Order n : n Product
    public ICollection<Product>? Products = [];
}