using System.Net;

namespace Sales.Application.ResultPattern;

public class UserErrors
{
    public static readonly Error DataIsNull = new Error("UserDataIsNull", "User must not be null.",HttpStatusCode.BadRequest);
    public static readonly Error IncorrectFormatData = new Error("UserInputNotValid", "User input is not in the correct format",HttpStatusCode.BadRequest);
    public static readonly Error NotFound = new Error("UserNotFound", "Past Id does not match any user", HttpStatusCode.NotFound);
    public static readonly Error CpfNotFound = new Error("UserNotFound", "Past CPF does not match any user", HttpStatusCode.NotFound);
    public static readonly Error IdMismatch = new Error("UserIdMismatch", "Past Id does not match user id", HttpStatusCode.BadRequest);
}