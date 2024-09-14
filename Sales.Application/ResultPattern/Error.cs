using System.Net;

namespace Sales.Application.ResultPattern;

public sealed record Error(string Code, string? Description = null, HttpStatusCode? HttpStatusCode = null)
{
    public static readonly Error None = new(string.Empty);
    
    // Create implicit operators for result pattern
}