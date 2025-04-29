using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.DTOs.ShoppingCartDTO;
using Sales.Application.Interfaces;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShoppingCartsController(IShoppingCartService _shoppingCartService) : ControllerBase
{
    [HttpGet("{userId:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<ShoppingCartDTOOutput>> GetShoppingCartByUserId(int userId)
    {
        var result = await _shoppingCartService.GetShoppingCartWithProductsAsync(userId);

        return result.isSuccess switch
        {
            true => Ok(result.value),
            false => NotFound(result.GenerateErrorResponse())
        };
    }
    
    [HttpGet("{userId:int:min(1)}/ProductsChecked")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<ShoppingCartDTOOutput>> GetShoppingCartWithProductsCheckedByUserId(int userId)
    {
        var result = await _shoppingCartService.GetShoppingCartWithProductsCheckedAsync(userId);

        return result.isSuccess switch
        {
            true => Ok(result.value),
            false => NotFound(result.GenerateErrorResponse())
        };
    }
    
    [HttpPost("{userId:int:min(1)}/AddProduct/{productId:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<ShoppingCartDTOOutput>> AddProductToShoppingCart(int userId, int productId, [FromQuery] decimal amount = 1)
    {
        var result = await _shoppingCartService.AddProductToShoppingCartAsync(userId, productId, amount);

        if (!result.isSuccess)
        {
            if(result.error.HttpStatusCode is HttpStatusCode.NotFound)
                return NotFound(result.GenerateErrorResponse());

            return BadRequest(result.GenerateErrorResponse());
        }
        
        return await GetShoppingCartByUserId(userId);
    }
    
    [HttpDelete("{userId:int:min(1)}/RemoveProduct/{productId:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<ShoppingCartDTOOutput>> RemoveProductFromShoppingCart(int userId, int productId)
    {
        var result = await _shoppingCartService.RemoveProductFromShoppingCartAsync(userId, productId);

        if (!result.isSuccess)
        {
            if(result.error.HttpStatusCode is HttpStatusCode.NotFound)
                return NotFound(result.GenerateErrorResponse());

            return BadRequest(result.GenerateErrorResponse());
        }
        
        return await GetShoppingCartByUserId(userId);
    }
    
    [HttpDelete("{userId:int:min(1)}/ClearCart")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<ShoppingCartDTOOutput>> ClearShoppingCart(int userId)
    {
        var result = await _shoppingCartService.ClearShoppingCartAsync(userId);

        if (!result.isSuccess)
        {
            if(result.error.HttpStatusCode is HttpStatusCode.NotFound)
                return NotFound(result.GenerateErrorResponse());

            return BadRequest(result.GenerateErrorResponse());
        }
        
        return await GetShoppingCartByUserId(userId);
    }
    
    [HttpPost("{userId:int:min(1)}/CheckProduct/{productId:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<ShoppingCartDTOOutput>> CheckProductFromShoppingCart(int userId, int productId)
    {
        var result = await _shoppingCartService.CheckProductFromShoppingCartAsync(userId, productId);

        if (!result.isSuccess)
        {
            if(result.error.HttpStatusCode is HttpStatusCode.NotFound)
                return NotFound(result.GenerateErrorResponse());

            return BadRequest(result.GenerateErrorResponse());
        }
        
        return await GetShoppingCartByUserId(userId);
    }
    
    [HttpPost("{userId:int:min(1)}/UncheckProduct/{productId:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<ShoppingCartDTOOutput>> UncheckProductFromShoppingCart(int userId, int productId)
    {
        var result = await _shoppingCartService.UncheckProductFromShoppingCartAsync(userId, productId);

        if (!result.isSuccess)
        {
            if(result.error.HttpStatusCode is HttpStatusCode.NotFound)
                return NotFound(result.GenerateErrorResponse());

            return BadRequest(result.GenerateErrorResponse());
        }
        
        return await GetShoppingCartByUserId(userId);
    }
    
    [HttpPut("{userId:int:min(1)}/UpdateProductAmount/{productId:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<ShoppingCartDTOOutput>> UpdateProductAmount(int userId, int productId, [FromQuery] decimal amount)
    {
        var result = await _shoppingCartService.UpdateProductAmount(userId, productId, amount);

        if (!result.isSuccess)
        {
            if(result.error.HttpStatusCode is HttpStatusCode.NotFound)
                return NotFound(result.GenerateErrorResponse());

            return BadRequest(result.GenerateErrorResponse());
        }
        
        return await GetShoppingCartByUserId(userId);
    }
}