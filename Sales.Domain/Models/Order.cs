using System.Collections.ObjectModel;
using Sales.Domain.Models.Enums;

namespace Sales.Domain.Models;

public sealed class Order
{
    public int OrderId { get; private set; }
    public decimal TotalValue { get; private set; }
    public DateTime OrderDate { get; private set; }
    public Status OrderStatus { get; private set; }
    public string Note { get; private set; }

    // Order n : 1 User
    public int UserId { get; private set; }
    public User? User { get; private set; } 
    
    // Order n : n Product
    public ICollection<LineItem>? LineItems { get; private set; } = new List<LineItem>();

    private Order()
    {
    }

    public Order(int orderId, decimal totalValue, DateTime orderDate, string note, int userId, Status orderStatus = Status.Preparing)
    {
        OrderId = orderId;
        TotalValue = totalValue;
        OrderDate = orderDate;
        OrderStatus = orderStatus;
        Note = note;
        UserId = userId;
    }

    public Order(decimal totalValue, DateTime orderDate, string note, int userId, Status orderStatus = Status.Preparing)
    {
        TotalValue = totalValue;
        OrderDate = orderDate;
        OrderStatus = orderStatus;
        Note = note;
        UserId = userId;
    }

    public void IncreaseValue(decimal value)
    {
        this.TotalValue += value;
    }
    
    public void DecreaseValue(decimal value)
    {
        this.TotalValue -= value;
    }
    
    public void SentOrder()
    {
        this.OrderStatus = Status.Sent;
    }

    public void FinishOrder()
    {
        this.OrderStatus = Status.Finished;
    }
}