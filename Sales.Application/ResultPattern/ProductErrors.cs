using System.Net;

namespace Sales.Application.ResultPattern;

public class ProductErrors
{
    public static readonly Error DataIsNull = new Error("ProductDataIsNull", "Product must not be null.",HttpStatusCode.BadRequest);
    public static readonly Error IncorrectFormatData = new Error("ProductInputNotValid", "Product input is not in the correct format",HttpStatusCode.BadRequest);
    public static readonly Error NotFound = new Error("ProductNotFound", "Past Id does not match any product", HttpStatusCode.NotFound);
    public static readonly Error IdMismatch = new Error("ProductIdMismatch", "Past Id does not match product id", HttpStatusCode.BadRequest);
    public static readonly Error StockUnavailable = new Error("ProductStockUnavailable", "Past product stock is no more available", HttpStatusCode.BadRequest);
}