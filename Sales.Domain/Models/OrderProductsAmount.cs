namespace Sales.Domain.Models;

public class OrderProductsAmount
{
    public Order Order { get; private set; }
    public List<ProductAmount> ProductAmount { get; private set; }

    public OrderProductsAmount(Order order, List<ProductAmount> product)
    {
        Order = order;
        ProductAmount = product;
    }
}