namespace Sales.Domain.Models;

public class ProductChecked
{
    public Product Product { get; private set; }
    public bool? Checked { get; private set; }
    public decimal? Amount { get; private set; }

    private ProductChecked()
    {
    }

    public ProductChecked(Product product, bool? @checked, decimal? amount)
    {
        this.Product = product;
        Checked = @checked;
        Amount = amount;
    }
}