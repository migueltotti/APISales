using System.Net;

namespace Sales.Application.ResultPattern;

public class WorkDayErrors
{
    public static readonly Error DataIsNull = new Error(
        "WorkDayDataIsNull", 
        "WorkDay must not be null.",
        HttpStatusCode.BadRequest);
    public static readonly Error IncorrectFormatData = new Error(
        "WorkDayInputNotValid", 
        "WorkDay input is not in the correct format",
        HttpStatusCode.BadRequest);
    public static readonly Error NotFound = new Error(
        "WorkDayNotFound", 
        "Past Id does not match any workDay", 
        HttpStatusCode.NotFound);
    public static readonly Error IdMismatch = new Error(
        "WorkDayIdMismatch", 
        "Past Id does not match workDay id", 
        HttpStatusCode.BadRequest);
    public static readonly Error EmployeeIdMismatch = new Error(
        "WorkDayEmployeeIdMismatch", 
        "Past EmployeeId does not match workDay EmployeeId", 
        HttpStatusCode.BadRequest);
    public static readonly Error RoutineExists = new Error(
        "WorkDayRoutineExists", 
        "WorkDay routine already exists.", 
        HttpStatusCode.BadRequest);
}