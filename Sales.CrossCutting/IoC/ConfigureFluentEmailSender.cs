using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sales.CrossCutting.IoC;

public static class ConfigureFluentEmailSender
{
    public static IServiceCollection RegisterFluentEmailSender(this IServiceCollection services)
    {
        // services
        //     .AddFluentEmail(configuration["EmailSettings:FromEmail"])
        //     .AddSmtpSender(
        //         host: configuration["EmailSettings:Host"], 
        //         port: configuration.GetValue<int>("EmailSettings:Port"),
        //         username: configuration["EmailSettings:FromEmail"],
        //         password: configuration["EmailSettings:Password"]
        //     );
        var fromEmail = Environment.GetEnvironmentVariable("FROM_EMAIL");
        var fromName = Environment.GetEnvironmentVariable("FROM_NAME");
        var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

        services.AddFluentEmail(fromEmail, fromName)
            .AddSendGridSender(sendGridApiKey);

        return services;    
    }
}