using Sales.Application.DTOs.ProductDTO;
using Sales.Application.DTOs.ShoppingCartDTO;
using Sales.Domain.Models;

namespace Sales.Application.Mapping.Extentions;

public static class ShoppingCartProductMapping
{
    public static ShoppingCartDTOOutput ToShoppingCartProductDto(this ShoppingCartProductInfo shoppingCartProduct)
    {
        var products = new List<ProductCheckedDTO>();
        shoppingCartProduct.Products.ForEach(p => 
            products.Add(new ProductCheckedDTO(
                new ProductDTOOutput(
                    p.Product.ProductId,
                    p.Product.Name,
                    p.Product.Description,
                    p.Product.Value,
                    p.Product.TypeValue,
                    p.Product.StockQuantity,
                    p.Product.ImageUrl,
                    p.Product.CategoryId
                ), 
                p.Checked,
                p.Amount)
            ));

        var shoppingCartDto = new ShoppingCartDTOOutput(
            shoppingCartProduct.ShoppingCart.ShoppingCartId,
            shoppingCartProduct.ShoppingCart.TotalValue,
            shoppingCartProduct.ShoppingCart.ProductsCount,
            shoppingCartProduct.ShoppingCart.UserId,
            products
        );
        
        return shoppingCartDto;
    }
    
    public static ShoppingCartProductInfo ToShoppingCartProduct(this ShoppingCartDTOOutput shoppingCartProductDto)
    {
        var products = new List<ProductChecked>();
        shoppingCartProductDto.Products.ForEach(p => 
            products.Add(new ProductChecked(
                new Product(
                    p.Product.ProductId,
                    p.Product.Name,
                    p.Product.Description,
                    p.Product.Value,
                    p.Product.TypeValue,
                    p.Product.ImageUrl,
                    p.Product.StockQuantity,
                    p.Product.CategoryId
                ), 
                p.Checked,
                p.Amount)
            ));

        var shoppingCart = new ShoppingCartProductInfo(
            new ShoppingCart(
                shoppingCartProductDto.ShoppingCartId,
                shoppingCartProductDto.TotalValue,
                shoppingCartProductDto.ProductsCount,
                shoppingCartProductDto.UserId
            ),
            products
        );
        
        return shoppingCart;
    }
}