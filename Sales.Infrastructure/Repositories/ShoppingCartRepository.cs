using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.EntityFrameworkCore;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Context;
using Sales.Infrastructure.DTO;

namespace Sales.Infrastructure.Repositories;

public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    public ShoppingCartRepository(SalesDbContext context) : base(context)
    {
    }

    public async Task<ShoppingCartProductInfo> GetShoppingCartWithItemsAsync(int userId)
    {
        var shoppingCart = await _context.Database.SqlQuery<ShoppingCartProductsCheckedDto>(
            $"""
             SELECT 
                 `s`.`ShoppingCartId`, 
                 `s`.`ProductsCount`, 
                 `s`.`TotalValue`, 
                 `s`.`UserId`, 
                 `p`.`ProductId`, 
                 `p`.`CategoryId`, 
                 `p`.`Description`, 
                 `p`.`ImageUrl`, 
                 `p`.`Name`, 
                 `p`.`StockQuantity`, 
                 `p`.`TypeValue`, 
                 `p`.`Value`, 
                 `s0`.`Checked`,
                 `s0`.`Amount`
             FROM `ShoppingCart` AS `s`
             LEFT JOIN `ShoppingCartProducts` AS `s0` ON `s`.`ShoppingCartId` = `s0`.`ShoppingCartId`
             LEFT JOIN `Product` AS `p` ON `s0`.`ProductId` = `p`.`ProductId`
             WHERE `s`.`UserId` = {userId}; 
             """
            ).ToListAsync();

        return GenerateShoppingCartWithProducts(shoppingCart);
    }

    public async Task<ShoppingCartProductInfo> GetShoppingCartWithItemsCheckedAsync(int userId)
    {
        var shoppingCart = await _context.Database.SqlQuery<ShoppingCartProductsCheckedDto>(
            $"""
             SELECT 
                 `s`.`ShoppingCartId`, 
                 `s`.`ProductsCount`, 
                 `s`.`TotalValue`, 
                 `s`.`UserId`, 
                 `p`.`ProductId`, 
                 `p`.`CategoryId`, 
                 `p`.`Description`, 
                 `p`.`ImageUrl`, 
                 `p`.`Name`, 
                 `p`.`StockQuantity`, 
                 `p`.`TypeValue`, 
                 `p`.`Value`, 
                 `s0`.`Checked`,
                 `s0`.`Amount`
             FROM `ShoppingCart` AS `s`
             LEFT JOIN `ShoppingCartProducts` AS `s0` ON `s`.`ShoppingCartId` = `s0`.`ShoppingCartId`
             LEFT JOIN `Product` AS `p` ON `s0`.`ProductId` = `p`.`ProductId`
             WHERE `s`.`UserId` = {userId} AND `s0`.`Checked` = 1; 
             """
        ).ToListAsync();

        return GenerateShoppingCartWithProducts(shoppingCart);
    }

    public async Task<ShoppingCartProduct?> GetProductOfShoppingCartAsync(int shoppingCartId, int productId)
    {
        var shoppingCartProduct = await _context.ShoppingCartProducts
            .AsNoTracking()
            .FirstOrDefaultAsync(shc => 
                shc.ShoppingCartId == shoppingCartId &&
                shc.ProductId == productId);

        return shoppingCartProduct;
    }

    public async Task AddItemToShoppingCartAsync(ShoppingCartProduct shoppingCartProduct)
    {
        var result = await _context.ShoppingCartProducts
            .AddAsync(shoppingCartProduct);
    }

    public void RemoveItemFromShoppingCartAsync(ShoppingCartProduct shoppingCartProduct)
    {
        var result = _context.ShoppingCartProducts
            .Remove(shoppingCartProduct);
    }

    public async Task<int> RemoveCheckedItemsFromShoppingCartAsync(int shoppingCartId)
    {
        var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
            $"""
             DELETE FROM `ShoppingCartProducts`
             WHERE `ShoppingCartId` = {shoppingCartId}
             AND `Checked` = 1;
             """
        );
        
        return rowsAffected;
    }

    public async Task<int> ClearShoppingCartAsync(int userId)
    {
        var rowsAffected = await _context.Database.ExecuteSqlAsync(
            $"""
                DELETE FROM `ShoppingCartProducts` AS `scp` 
                 WHERE `scp`.`ShoppingCartId` = (
             	    SELECT `sc`.`ShoppingCartId`
             	    FROM `ShoppingCart` AS `sc`
             	    WHERE `sc`.`UserId` = 2
                 );
             """    
        );

        return rowsAffected;
    }

    public async Task<int> CheckItemFromShoppingCartAsync(ShoppingCartProduct shoppingCartProduct)
    {
        var rowsAffected = await _context.Database.ExecuteSqlAsync(
            $"""
              UPDATE shoppingcartproducts s
              SET checked = 1
              WHERE s.shoppingcartid = {shoppingCartProduct.ShoppingCartId}
              AND s.productid = {shoppingCartProduct.ProductId};
              """
        );

        return rowsAffected;
    }

    public async Task<int> UncheckItemFromShoppingCartAsync(ShoppingCartProduct shoppingCartProduct)
    {
        var rowsAffected = await _context.Database.ExecuteSqlAsync(
            $"""
             UPDATE shoppingcartproducts s
             SET checked = 0
             WHERE s.shoppingcartid = {shoppingCartProduct.ShoppingCartId}
               AND s.productid = {shoppingCartProduct.ProductId};
             """
        );

        return rowsAffected;
    }
    
    /////////////////////////////////// PRIVATE METHODS ///////////////////////////////////

    
    private static ShoppingCartProductInfo GenerateShoppingCartWithProducts(List<ShoppingCartProductsCheckedDto> shoppingCart)
    {
        List<ProductChecked> products = [];
        shoppingCart.ForEach(shc => products.Add( 
            new ProductChecked(
                new Product(
                    shc.ProductId ?? 0,
                    shc.Name ?? string.Empty,
                    shc.Description ?? string.Empty,
                    shc.Value ?? 0.0m,
                    shc.TypeValue ?? 0,
                    shc.ImageUrl ?? string.Empty,
                    shc.StockQuantity ?? 0,
                    shc.CategoryId ?? 0
                ), 
                shc.Checked,
                shc.Amount
            )
        ));
        
        return new ShoppingCartProductInfo(
            new ShoppingCart(
                shoppingCart[0].ShoppingCartId!.Value,
                shoppingCart[0].TotalValue!.Value,
                shoppingCart[0].ProductsCount!.Value,
                shoppingCart[0].UserId!.Value
            ),
            products!
        );
    }
}