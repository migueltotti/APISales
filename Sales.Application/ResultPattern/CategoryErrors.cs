using System.Net;

namespace Sales.Application.ResultPattern;

public class CategoryErrors
{
    public static readonly Error DataIsNull = new Error("CategoryDataIsNull", "Category must not be null.",HttpStatusCode.BadRequest);
    public static readonly Error IncorrectFormatData = new Error("CategoryInputNotValid", "Category input is not in the correct format",HttpStatusCode.BadRequest);
    public static readonly Error NotFound = new Error("CategoryNotFound", "Past Id does not match any category", HttpStatusCode.NotFound);
    public static readonly Error IdMismatch = new Error("CategoryIdMismatch", "Past Id does not match category id", HttpStatusCode.BadRequest);
}