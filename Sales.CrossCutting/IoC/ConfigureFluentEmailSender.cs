using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sales.CrossCutting.IoC;

public static class ConfigureFluentEmailSender
{
    public static IServiceCollection RegisterFluentEmailSender(this IServiceCollection services, IConfiguration configuration)
    {
        // services
        //     .AddFluentEmail(configuration["EmailSettings:FromEmail"])
        //     .AddSmtpSender(
        //         host: configuration["EmailSettings:Host"], 
        //         port: configuration.GetValue<int>("EmailSettings:Port"),
        //         username: configuration["EmailSettings:FromEmail"],
        //         password: configuration["EmailSettings:Password"]
        //     );

        services.AddFluentEmail(configuration["FROM_EMAIL"], configuration["FROM_NAME"])
            .AddSendGridSender(configuration["SENDGRID_API_KEY"]);

        return services;
    }
}