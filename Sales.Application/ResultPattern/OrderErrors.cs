using System.Net;

namespace Sales.Application.ResultPattern;

public class OrderErrors
{
    public static readonly Error DataIsNull = new Error("OrderDataIsNull", "Order must not be null.",HttpStatusCode.BadRequest);
    public static readonly Error IncorrectFormatData = new Error("OrderInputNotValid", "Order input is not in the correct format",HttpStatusCode.BadRequest);
    public static readonly Error NotFound = new Error("OrderNotFound", "Past Id does not match any order", HttpStatusCode.NotFound);
    public static readonly Error IdMismatch = new Error("OrderIdMismatch", "Past Id does not match order id", HttpStatusCode.BadRequest);
}