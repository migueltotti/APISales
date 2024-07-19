using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiSales.ExceptionHandler;

public class ControllersExceptionFilter(ILogger<ControllersExceptionFilter> _logger) : IExceptionFilter
{
    
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception occurred: Status code 500");

        context.Result = new ObjectResult("An error occurred while treating your request: Status code 500")
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}