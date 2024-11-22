namespace Sales.Domain.Models;

public class ShoppingCart
{
    public int ShoppingCartId { get; private set; }
    public double TotalValue { get; private set; }
    // TODO: add partial value
    // correspond to the sum of all checked products
    // totalValue: sum of all products
    public int ProductsCount { get; private set; }
    
    // ShoppingCart 1 : 1 User (userId -> UNIQUE) 
    public int UserId { get; private set; }
    public User? User { get; private set; }

    // ShoppingCart n : n Products
    public ICollection<Product>? Products { get; set; }

    private ShoppingCart()
    {
    }

    public ShoppingCart(double totalValue, int productsCount, int userId)
    {
        TotalValue = totalValue;
        ProductsCount = productsCount;
        UserId = userId;
        Products = new List<Product>();
    }

    public ShoppingCart(int shoppingCartId, double totalValue, int productsCount, int userId)
    {
        ShoppingCartId = shoppingCartId;
        TotalValue = totalValue;
        ProductsCount = productsCount;
        UserId = userId;
        Products = new List<Product>();
    }

    public ShoppingCart(int shoppingCartId, double totalValue, int productsCount, int userId, User? user, ICollection<Product>? products)
    {
        ShoppingCartId = shoppingCartId;
        TotalValue = totalValue;
        ProductsCount = productsCount;
        UserId = userId;
        User = user;
        Products = products;
    }
  
    public void IncreaseProductCount()
    {
        ProductsCount++;
    }
    
    public void DecreaseProductCount()
    {
        if(ProductsCount == 0)
            throw new InvalidOperationException("No products have been increased");
            
        ProductsCount--;
    }
    
    public void DecreaseProductCount(int productsCount)
    {
        if(ProductsCount == 0)
            throw new InvalidOperationException("No products have been increased");
        
        if(ProductsCount < productsCount)
            throw new ArgumentException("Products count cannot be less than products count");
            
        ProductsCount -= productsCount;
    }
    
    public void IncreaseTotalValue(double productValue, double productAmount)
    {
        TotalValue += (productAmount * productValue);
    }

    public void DecreaseTotalValue(double productValue, double productAmount)
    {
        TotalValue -= (productAmount * productValue);
    }
    
    public void DecreaseTotalValue(double totalAmount)
    {
        if(totalAmount > TotalValue)
            throw new ArgumentException("The total amount cannot be greater than the total value.");
        
        TotalValue -= totalAmount;
    }
}