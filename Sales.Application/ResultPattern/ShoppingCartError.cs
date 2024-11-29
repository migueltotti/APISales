using System.Net;

namespace Sales.Application.ResultPattern;

public class ShoppingCartError
{
    public static readonly Error IncorrectFormatData = new Error(
        Code: "ShoppingCartInputNotValid",
        Description: "ShoppingCart input is not in the correct format",
        HttpStatusCode.BadRequest);
    
    public static readonly Error NotFound = new Error(
        Code: "ShoppingCartNotFound", 
        Description: "Past Id does not match any shoppingCart", 
        HttpStatusCode.NotFound);
    
    public static readonly Error ProductNotFound = new Error(
        Code: "ShoppingCartProductNotFound", 
        Description: "Past productId does not match any product from shoppingCart", 
        HttpStatusCode.NotFound);
    
    public static readonly Error DuplicateData = new Error(
        Code: "ShoppingCartDuplicateData", 
        Description: "Past shoppingCart with the same userId already exists in stock", 
        HttpStatusCode.BadRequest);

    public static readonly Error CreateError = new Error(
        Code: "ShoppingCartCreateError", 
        Description: "An error occured while creating the shopping cart. Try again later.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error DeleteError = new Error(
        Code: "ShoppingCartDeleteError", 
        Description: "An error occured while deleting the shopping cart. Try again later.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error AddItemError = new Error(
        Code: "ShoppingCartAddItemError", 
        Description: "An error occured while trying to add a item to the shopping cart. Try again later.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error RemoveItemError = new Error(
        Code: "ShoppingCartRemoveItemError", 
        Description: "An error occured while trying to remove a item from the shopping cart. Try again later.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error RemoveCheckedItemsError = new Error(
        Code: "ShoppingCartRemoveCheckedItemError", 
        Description: "An error occured while trying to remove all checked item from the shopping cart. Try again later.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error UpdateTotalValueAndProductCountItemsError = new Error(
        Code: "ShoppingCartUpdateTotalValueAndProductCountItemsError", 
        Description: "An error occured while trying to remove all checked item from the shopping cart. Try again later.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error UpdateTotalValueError = new Error(
        Code: "ShoppingCartUpdateTotalValueErrorError", 
        Description: "An error occured while trying to update total value from the shopping cart. Try again later.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error UpdateProductAmountError = new Error(
        Code: "ShoppingCartUpdateProductAmountError", 
        Description: "An error occured while trying to update product amount from the shopping cart. Try again later.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error ClearAllItemsError = new Error(
        Code: "ShoppingCartClearAllItemsError", 
        Description: "An error occured while trying to clear the shopping cart. Try again later.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error CheckItemError = new Error(
        Code: "ShoppingCartCheckItemError", 
        Description: "An error occured while trying to check a item of the shopping cart. Try again later.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error UncheckItemError = new Error(
        Code: "ShoppingCartUncheckItemError", 
        Description: "An error occured while trying to uncheck a item of the shopping cart. Try again later.", 
        HttpStatusCode.BadRequest);
}