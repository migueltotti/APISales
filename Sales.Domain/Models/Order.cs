using System.Collections.ObjectModel;

namespace Sales.Domain.Models;

public sealed class Order
{
    public int OrderId { get; private set; }
    public decimal TotalValue { get; private set; }
    public DateTime OrderDate { get; private set; }

    // Order n : 1 User
    public int UserId { get; private set; }
    public User? User { get; private set; } 
    
    // Order n : n Product
    public ICollection<Product>? Products { get; private set; }

    public Order(int orderId, decimal totalValue, DateTime orderDate, int userId)
    {
        OrderId = orderId;
        TotalValue = totalValue;
        OrderDate = orderDate;
        UserId = userId;
    }

    public Order(decimal totalValue, DateTime orderDate, int userId)
    {
        TotalValue = totalValue;
        OrderDate = orderDate;
        UserId = userId;
    }
}