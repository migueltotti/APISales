using System.Net;

namespace Sales.Application.ResultPattern;

public class AffiliateErros
{
    public static readonly Error DataIsNull = new Error("AffiliateDataIsNull", "Affiliate must not be null.",HttpStatusCode.BadRequest);
    public static readonly Error IncorrectFormatData = new Error("AffiliateInputNotValid", "Affiliate input is not in the correct format",HttpStatusCode.BadRequest);
    public static readonly Error NotFound = new Error("AffiliateNotFound", "Past Id does not match any affiliate", HttpStatusCode.NotFound);
    public static readonly Error IdMismatch = new Error("AffiliateIdMismatch", "Past Id does not match affiliate id", HttpStatusCode.BadRequest);
    public static readonly Error DuplicateData = new Error("AffiliateDuplicateData", "Past affiliate with the same name already exists in stock", HttpStatusCode.BadRequest);

}