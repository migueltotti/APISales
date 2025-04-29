using System.Net;

namespace Sales.Application.ResultPattern;

public class UserErrors
{
    public static readonly Error DataIsNull = new Error(
        "UserDataIsNull", 
        "User must not be null.",
        HttpStatusCode.BadRequest);
    
    public static readonly Error IncorrectFormatData = new Error(
        "UserInputNotValid", 
        "User input is not in the correct format",
        HttpStatusCode.BadRequest);
    
    public static readonly Error NotFound = new Error(
        "UserNotFound", 
        "Past Id does not match any user", 
        HttpStatusCode.NotFound);
    
    public static readonly Error CpfNotFound = new Error(
        "UserNotFound", 
        "Past CPF does not match any user", 
        HttpStatusCode.NotFound);
    
    public static readonly Error IdMismatch = new Error(
        "UserIdMismatch", 
        "Past Id does not match user id", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error UserExists = new Error(
        "UserEmailExists", 
        "User with the same Email already exists", 
        HttpStatusCode.InternalServerError);
    
    public static readonly Error PasswordMismatch = new Error(
        "OldPasswordMismatch", 
        "Old password passed in request does not match old password from User!", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error PasswordsEqualError = new Error(
        "NewPasswordEqualsOldPassword", 
        "New Password must be different from the Old Password!", 
        HttpStatusCode.BadRequest);
    
    public static readonly Error PasswordChangeError = new Error(
        "PasswordChangError", 
        "An error occured while trying to change password!", 
        HttpStatusCode.InternalServerError);
}