using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sales.API.Filter;

public class LoggingRequestsFilter : IActionFilter
{
    private readonly ILogger<LoggingRequestsFilter> _logger;

    public LoggingRequestsFilter(ILogger<LoggingRequestsFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation(
            "Starting request {@RequestName}, {@DateTime}",
            context.ActionDescriptor.DisplayName,
            DateTime.Now
        );
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation(
            "Completed request {@RequestName}, {@DateTime}",
            context.ActionDescriptor.DisplayName,
            DateTime.Now
        );
    }
}