namespace Sales.Domain.Models;

public class ShoppingCartProduct
{
    public int ShoppingCartId { get; private set; }
    public int ProductId { get; private set; }
    public bool Checked { get; private set; }
    public decimal Amount { get; private set; } 

    private ShoppingCartProduct()
    { }

    public ShoppingCartProduct(int shoppingCartId, int productId, bool @checked = true)
    {
        ShoppingCartId = shoppingCartId;
        ProductId = productId;
        Checked = @checked;
    }

    public ShoppingCartProduct(int shoppingCartId, int productId, decimal amount, bool @checked = true)
    {
        ShoppingCartId = shoppingCartId;
        ProductId = productId;
        Checked = @checked;
        Amount = amount;
    }

    public void CheckProduct() => Checked = true;
    
    public void UnCheckProduct() => Checked = false;
}