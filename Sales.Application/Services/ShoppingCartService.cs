using AutoMapper;
using Sales.Application.DTOs.ShoppingCartDTO;
using Sales.Application.Interfaces;
using Sales.Application.Mapping.Extentions;
using Sales.Application.ResultPattern;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.Application.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IUnitOfWork _uof;
    private readonly IUserService _userService;
    private readonly IProductService _productService;

    public ShoppingCartService(IUnitOfWork uof, IUserService userService, IProductService productService)
    {
        _uof = uof;
        _userService = userService;
        _productService = productService;
    }

    public async Task<Result<ShoppingCartDTOOutput>> GetShoppingCartWithProductsAsync(int userId)
    {
        var userResult = await _userService.GetUserById(userId);

        if (!userResult.isSuccess)
            return Result<ShoppingCartDTOOutput>.Failure(userResult.error);

        var shoppingCartExists = await _uof.ShoppingCartRepository.GetAsync(sh => sh.UserId == userId);

        if (shoppingCartExists is null)
            return Result<ShoppingCartDTOOutput>.Failure(ShoppingCartError.NotFound);
        
        var shoppingCart = await _uof.ShoppingCartRepository.GetShoppingCartWithItemsAsync(userId);

        if (shoppingCart is null)
            return Result<ShoppingCartDTOOutput>.Failure(ShoppingCartError.NotFound);

        var shoppingCartDto = shoppingCart.ToShoppingCartProductDto();
        
        return Result<ShoppingCartDTOOutput>.Success(shoppingCartDto);
    }

    public async Task<Result<ShoppingCartDTOOutput>> GetShoppingCartWithProductsCheckedAsync(int userId)
    {
        var userResult = await _userService.GetUserById(userId);

        if (!userResult.isSuccess)
            return Result<ShoppingCartDTOOutput>.Failure(userResult.error);

        var shoppingCartExists = await _uof.ShoppingCartRepository.GetAsync(sh => sh.UserId == userId);

        if (shoppingCartExists is null)
            return Result<ShoppingCartDTOOutput>.Failure(ShoppingCartError.NotFound);
        
        var shoppingCartProductsChecked = await _uof.ShoppingCartRepository.GetShoppingCartWithItemsCheckedAsync(userId);

        if (shoppingCartProductsChecked is null)
            return Result<ShoppingCartDTOOutput>.Failure(ShoppingCartError.NotFound);

        var shoppingCartDto = shoppingCartProductsChecked.ToShoppingCartProductDto();
        
        return Result<ShoppingCartDTOOutput>.Success(shoppingCartDto);
    }

    public async Task<Result<ShoppingCartDTOOutput>> CreateShoppingCartAsync(int userId)
    {
        var userResult = await _userService.GetUserById(userId);
        
        if (!userResult.isSuccess)
            return Result<ShoppingCartDTOOutput>.Failure(userResult.error);
        
        var shoppingCartExists = await _uof.ShoppingCartRepository
            .GetAsync(sh => sh.UserId == userId);

        if (shoppingCartExists is not null)
            return Result<ShoppingCartDTOOutput>.Failure(ShoppingCartError.DuplicateData);

        var newShoppingCart = new ShoppingCart(0, 0, userId);
        
        var result = _uof.ShoppingCartRepository.Create(newShoppingCart);
        await _uof.CommitChanges();

        if (result is null)
            return Result<ShoppingCartDTOOutput>.Failure(ShoppingCartError.CreateError);
        
        var shoppingCartDTO = new ShoppingCartDTOOutput(
                newShoppingCart.ShoppingCartId,
                newShoppingCart.TotalValue,
                newShoppingCart.ProductsCount,
                newShoppingCart.UserId,
                []
            );
        
        return Result<ShoppingCartDTOOutput>.Success(shoppingCartDTO);
    }

    public async Task<Result<bool>> DeleteShoppingCartAsync(int userId)
    {
        var userResult = await _userService.GetUserById(userId);
        
        if (!userResult.isSuccess)
            return Result<bool>.Failure(userResult.error);
        
        var shoppingCart = await _uof.ShoppingCartRepository.GetAsync(sc => sc.UserId == userId);
        
        if(shoppingCart is null)
            return Result<bool>.Failure(ShoppingCartError.NotFound);

        var result = _uof.ShoppingCartRepository.Delete(shoppingCart);
        await _uof.CommitChanges();
        
        if(result is null)
            return Result<bool>.Failure(ShoppingCartError.DeleteError);
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> AddProductToShoppingCartAsync(int userId, int productId, decimal amount)
    {
        // verify user
        var userResult = await _userService.GetUserById(userId);

        if (!userResult.isSuccess)
            return Result<bool>.Failure(userResult.error);
        
        // get shoppingCart info
        var shoppingCartResult = await _uof.ShoppingCartRepository
            .GetAsync(sh => sh.UserId == userResult.value!.UserId);
        
        if (shoppingCartResult is null)
            return Result<bool>.Failure(ShoppingCartError.NotFound);

        // verify product
        var prodResult = await _productService.GetProductById(productId);
        
        if (!prodResult.isSuccess)
            return Result<bool>.Failure(prodResult.error);
        
        if(prodResult.value!.StockQuantity == 0)
            return Result<bool>.Failure(ProductErrors.StockUnavailable); 
        
        // add product to shoppingCart
        var shoppingCartProductToAdd = new ShoppingCartProduct(
            shoppingCartResult.ShoppingCartId,
            prodResult.value.ProductId,
            amount
        );
        
        // TODO: increase total value from shopping cart
        // based on product amount and product value

        shoppingCartResult.IncreaseProductCount();
        shoppingCartResult.IncreaseTotalValue(
            (double) prodResult.value!.Value, 
            (double) amount 
        );
        
        try
        {
            await _uof.ShoppingCartRepository.AddItemToShoppingCartAsync(shoppingCartProductToAdd);
            _uof.ShoppingCartRepository.Update(shoppingCartResult);
            await _uof.CommitChanges();
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ShoppingCartError.AddItemError); 
        }
        
        return Result<bool>.Success(true);        
    }

    public async Task<Result<bool>> RemoveProductFromShoppingCartAsync(int userId, int productId)
    {
        // verify user
        var userResult = await _userService.GetUserById(userId);

        if (!userResult.isSuccess)
            return Result<bool>.Failure(userResult.error);
        
        // get shoppingCart info
        var shoppingCartResult = await _uof.ShoppingCartRepository
            .GetAsync(sh => sh.UserId == userResult.value!.UserId);
        
        if (shoppingCartResult is null)
            return Result<bool>.Failure(ShoppingCartError.NotFound);

        // verify product
        var prodResult = await _productService.GetProductById(productId);
        
        if (!prodResult.isSuccess)
            return Result<bool>.Failure(prodResult.error);
        
        // remove product to shoppingCart
        var shoppingCartProductToRemove = new ShoppingCartProduct(
            shoppingCartResult.ShoppingCartId,
            prodResult.value.ProductId
        );

        // decrease value based on product value and amount
        var productShoppingCart = await _uof.ShoppingCartRepository
            .GetProductOfShoppingCartAsync(shoppingCartResult.ShoppingCartId, productId);
        
        if(productShoppingCart is null)
            return Result<bool>.Failure(ShoppingCartError.ProductNotFound);

        shoppingCartResult.DecreaseProductCount();
        shoppingCartResult.DecreaseTotalValue(
            (double) prodResult.value!.Value, 
            (double) productShoppingCart.Amount 
        );

        try
        {
            _uof.ShoppingCartRepository.RemoveItemFromShoppingCartAsync(shoppingCartProductToRemove);
            _uof.ShoppingCartRepository.Update(shoppingCartResult);
            await _uof.CommitChanges();
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ShoppingCartError.RemoveItemError); 
        }
        
        return Result<bool>.Success(true); 
    }

    public async Task<Result<bool>> ClearShoppingCartAsync(int userId)
    {
        // verify user
        var userResult = await _userService.GetUserById(userId);

        if (!userResult.isSuccess)
            return Result<bool>.Failure(userResult.error);

        // get shoppingCart info
        var shoppingCartResult = await _uof.ShoppingCartRepository
            .GetAsync(sh => sh.UserId == userResult.value!.UserId);

        if (shoppingCartResult is null)
            return Result<bool>.Failure(ShoppingCartError.NotFound);

        var shoppingCartClear = new ShoppingCart(
            shoppingCartResult.ShoppingCartId,
            0,
            0,
            shoppingCartResult.UserId
        );
        
        var rowsAffected = await _uof.ShoppingCartRepository.ClearShoppingCartAsync(userResult.value!.UserId);
        var shoppingCartClearResult = _uof.ShoppingCartRepository.Update(shoppingCartClear);
        await _uof.CommitChanges();

        if(rowsAffected == 0 || shoppingCartClearResult is null)
            return Result<bool>.Failure(ShoppingCartError.ClearAllItemsError); 
        
        return Result<bool>.Success(true);  
    }

    public async Task<Result<bool>> CheckProductFromShoppingCartAsync(int userId, int productId)
    {
        // verify user
        var userResult = await _userService.GetUserById(userId);

        if (!userResult.isSuccess)
            return Result<bool>.Failure(userResult.error);
        
        // get shoppingCart info
        var shoppingCartResult = await _uof.ShoppingCartRepository
            .GetAsync(sh => sh.UserId == userResult.value!.UserId);
        
        if (shoppingCartResult is null)
            return Result<bool>.Failure(ShoppingCartError.NotFound);

        // verify product
        var prodResult = await _productService.GetProductById(productId);
        
        if (!prodResult.isSuccess)
            return Result<bool>.Failure(prodResult.error);
        
        // verify if product exists in shopping cart
        var shoppingCartProduct = await _uof.ShoppingCartRepository
            .GetProductOfShoppingCartAsync(shoppingCartResult.ShoppingCartId, productId);
        
        if(shoppingCartProduct is null)
            return Result<bool>.Failure(ShoppingCartError.ProductNotFound);
        
        // check product to shoppingCart
        var shoppingCartProductToCheck = new ShoppingCartProduct(
            shoppingCartResult.ShoppingCartId,
            prodResult.value.ProductId
        );
        
        var rowsAffected = await _uof.ShoppingCartRepository.CheckItemFromShoppingCartAsync(shoppingCartProductToCheck);
        
        // Update total value based on Product value and amount 
        shoppingCartResult.IncreaseTotalValue(
            (double) prodResult.value.Value,
            (double) shoppingCartProduct.Amount
        );
        
        var shoppingCartIncreasedTotalValue = _uof.ShoppingCartRepository.Update(shoppingCartResult); 
        await _uof.CommitChanges();

        if (shoppingCartIncreasedTotalValue is null)
            return Result<bool>.Failure(ShoppingCartError.UpdateTotalValueError);
        
        await _uof.CommitChanges();
    
        if(rowsAffected == 0)
            return Result<bool>.Failure(ShoppingCartError.CheckItemError);  
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UncheckProductFromShoppingCartAsync(int userId, int productId)
    {
        // verify user
        var userResult = await _userService.GetUserById(userId);

        if (!userResult.isSuccess)
            return Result<bool>.Failure(userResult.error);
        
        // get shoppingCart info
        var shoppingCartResult = await _uof.ShoppingCartRepository
            .GetAsync(sh => sh.UserId == userResult.value!.UserId);
        
        if (shoppingCartResult is null)
            return Result<bool>.Failure(ShoppingCartError.NotFound);

        // verify product
        var prodResult = await _productService.GetProductById(productId);
        
        if (!prodResult.isSuccess)
            return Result<bool>.Failure(prodResult.error);

        // verify if product exists in shopping cart
        var shoppingCartProduct = await _uof.ShoppingCartRepository
            .GetProductOfShoppingCartAsync(shoppingCartResult.ShoppingCartId, productId);
        
        if(shoppingCartProduct is null)
            return Result<bool>.Failure(ShoppingCartError.ProductNotFound);

        // uncheck product to shoppingCart

        var rowsAffected = await _uof.ShoppingCartRepository.UncheckItemFromShoppingCartAsync(shoppingCartProduct);
        
        if(rowsAffected == 0)
                    return Result<bool>.Failure(ShoppingCartError.UncheckItemError);
        
        // Decrease TotalValue based on Product unchecked value and amount
        shoppingCartResult.DecreaseTotalValue(
            (double) prodResult.value.Value,
            (double) shoppingCartProduct.Amount
        );
        
        var shoppingCartDecreasedTotalValue = _uof.ShoppingCartRepository.Update(shoppingCartResult); 
        await _uof.CommitChanges();

        if (shoppingCartDecreasedTotalValue is null)
            return Result<bool>.Failure(ShoppingCartError.UpdateTotalValueError);

        return Result<bool>.Success(true);
    }
}