
namespace Sales.Domain.Models;

public class ShoppingCartProductInfo
{
    public ShoppingCart ShoppingCart { get; private set; }
    public List<ProductChecked> Products { get; private set; }

    private ShoppingCartProductInfo()
    {
    }

    public ShoppingCartProductInfo(ShoppingCart shoppingCart,  List<ProductChecked> products)
    {
        ShoppingCart = shoppingCart;
        Products = products;
    }
}