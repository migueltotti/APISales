using Sales.Domain.Models;

namespace Sales.Domain.Interfaces;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    Task<ShoppingCartProductInfo> GetShoppingCartWithItemsAsync(int userId);
    Task<ShoppingCartProductInfo> GetShoppingCartWithItemsCheckedAsync(int userId);
    Task<ShoppingCartProduct?> GetProductOfShoppingCartAsync(int shoppingCartId, int productId);
    Task AddItemToShoppingCartAsync(ShoppingCartProduct shoppingCartProduct);
    void RemoveItemFromShoppingCartAsync(ShoppingCartProduct shoppingCartProduct);
    Task<int> RemoveCheckedItemsFromShoppingCartAsync(int shoppingCartId);
    Task<int> ClearShoppingCartAsync(int userId);
    Task<int> CheckItemFromShoppingCartAsync(ShoppingCartProduct shoppingCartProduct);
    Task<int> UncheckItemFromShoppingCartAsync(ShoppingCartProduct shoppingCartProduct);
}