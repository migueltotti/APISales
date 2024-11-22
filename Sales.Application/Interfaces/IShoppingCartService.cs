using Sales.Application.DTOs.ShoppingCartDTO;
using Sales.Application.ResultPattern;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.Application.Interfaces;

public interface IShoppingCartService
{
    Task<Result<ShoppingCartDTOOutput>> GetShoppingCartWithProductsAsync(int userId);
    Task<Result<ShoppingCartDTOOutput>> GetShoppingCartWithProductsCheckedAsync(int userId);
    Task<Result<ShoppingCartDTOOutput>> CreateShoppingCartAsync(int userId);
    Task<Result<bool>> DeleteShoppingCartAsync(int userId);
    Task<Result<bool>> AddProductToShoppingCartAsync(int userId, int productId, decimal amount);
    Task<Result<bool>> RemoveProductFromShoppingCartAsync(int userId, int productId);
    Task<Result<bool>> ClearShoppingCartAsync(int userId);
    Task<Result<bool>> CheckProductFromShoppingCartAsync(int userId, int productId);
    Task<Result<bool>> UncheckProductFromShoppingCartAsync(int userId, int productId);
}