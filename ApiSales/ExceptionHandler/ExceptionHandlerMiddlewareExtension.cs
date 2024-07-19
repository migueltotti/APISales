using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace ApiSales.ExceptionHandler;

public static class ExceptionHandlerMiddlewareExtension
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (context.Features is not null)
                {
                    await context.Response.WriteAsync(new ErrorDetail()
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = contextFeature.Error.Message,
                        Trace = contextFeature.Error.StackTrace
                    }.ToString());
                }
            });
        });
    }
}