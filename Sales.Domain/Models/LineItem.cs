namespace Sales.Domain.Models;

public class LineItem
{
    public int LineItemId { get; private set; }
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal Price { get; private set; }

    public Product Product { get; private set; }
    public Order Order { get; private set; }

    private LineItem(){}

    public LineItem(int orderId, int productId, decimal amount, decimal price)
    {
        OrderId = orderId;
        ProductId = productId;
        Amount = amount;
        Price = price;
    }

    public LineItem(int lineItemId, int orderId, int productId, decimal amount, decimal price)
    {
        LineItemId = lineItemId;
        OrderId = orderId;
        ProductId = productId;
        Amount = amount;
        Price = price;
    }

    public void AddProduct(Product product)
    {
        Product = product;
    }
}