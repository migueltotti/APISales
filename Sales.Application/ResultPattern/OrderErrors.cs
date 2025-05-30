using System.Net;

namespace Sales.Application.ResultPattern;

public class OrderErrors
{
    public static readonly Error DataIsNull = new Error("OrderDataIsNull", "Order must not be null.",HttpStatusCode.BadRequest);
    public static readonly Error OrderFinishedOrSent = new Error("OrderFinished", "Order has already been finished or sent.",HttpStatusCode.BadRequest);
    public static readonly Error OrderFinished = new Error("OrderFinished", "Order has already been finished.",HttpStatusCode.BadRequest);
    public static readonly Error OrderSent = new Error("OrderSent", "Order has already been sent.",HttpStatusCode.BadRequest);
    public static readonly Error OrderNotSent = new Error("OrderNotSent", "Order has not been sent.",HttpStatusCode.BadRequest);
    public static readonly Error NoRowsAffected = new Error("NoRowsAffected", "Error while trying to add product to Order.",HttpStatusCode.BadRequest);
    public static readonly Error IncorrectFormatData = new Error("OrderInputNotValid", "Order input is not in the correct format.",HttpStatusCode.BadRequest);
    public static readonly Error NotFound = new Error("OrderNotFound", "Past Id does not match any order.", HttpStatusCode.NotFound);
    public static readonly Error ProductsNotFound = new Error("OrderProductsNotFound", "There`r no Products cadastred to this Order.", HttpStatusCode.NotFound);
    public static readonly Error ProductNotFound = new Error("OrderProductNotFound", "Past Id does not match any Product cadastred in Order.", HttpStatusCode.NotFound);
    
    public static readonly Error ProductListEmpty = new Error(
        "OrderProductListEmpty", 
        "Order must have at least one product cadastred.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error ProductsStockUnavailable = new Error("ProductsStockFail", "These products are no longer available in the system.", HttpStatusCode.NotFound);
    public static readonly Error IdMismatch = new Error("OrderIdMismatch", "Past Id does not match order id.", HttpStatusCode.BadRequest);
    
    public static readonly Error AddRangeError = new Error(
        "OrderAddRangeError", 
        "Error occured while adding a range of products to Order.", 
        HttpStatusCode.InternalServerError);
    
    public static readonly Error SinceNullParameter = new Error(
        "OrderSinceNullParameter", 
        "Parameter of type since must not be null.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error DateNullParameter = new Error(
        "OrderDateNullParameter", 
        "Parameter of type date must not be null and email must not be empty.", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error InvalidEmail = new Error(
        "OrderReportInvalidEmail", 
        "Email must be in valid format.", 
        HttpStatusCode.BadRequest);
}