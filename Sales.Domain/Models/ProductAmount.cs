namespace Sales.Domain.Models;

public class ProductAmount
{
    public Product Product { get; private set; }
    public decimal Amount { get; private set; }

    public ProductAmount(Product product, decimal amount)
    {
        Product = product;
        Amount = amount;
    }
}