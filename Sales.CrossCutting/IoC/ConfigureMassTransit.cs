using MassTransit;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Sales.Application.MassTransit.GenerateReport;

namespace Sales.CrossCutting.IoC;

public static class ConfigureMassTransit
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitConnectionString = configuration.GetConnectionString("RabbitMQ") ??
                                     throw new NullReferenceException("RabbitMQ connection string is null");
        
        services.AddMassTransit(busConfigurator =>
        {
            // add consumer
            busConfigurator.AddConsumer<GeneratePOSReportConsumer>();
            
            // configure RabbitMQ
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(new Uri(rabbitConnectionString), host =>
                {
                    host.Username("guest");
                    host.Password("guest");
                });

                // configure queue and exchange
                cfg.ReceiveEndpoint("pos-report-queue", queue =>
                {
                    queue.ConfigureConsumer<GeneratePOSReportConsumer>(ctx);
                    
                    queue.Bind("report-generator", bind =>
                    {
                        bind.ExchangeType = ExchangeType.Direct;
                        bind.RoutingKey = "report";
                    });
                });
                
            });
        });
        
        return services;
    }
}