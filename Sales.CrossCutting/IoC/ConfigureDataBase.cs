using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Infrastructure.Context;
using Sales.Infrastructure.Identity;

namespace Sales.CrossCutting.IoC;

public static class ConfigureDataBase
{
    public static IServiceCollection AddDataBase(this IServiceCollection services, IConfiguration configuration)
    {
        var salesDbConnectionString = Environment.GetEnvironmentVariable("SALES_CONNECTION_STRING") ?? throw new NullReferenceException("Sales Db connection string is null");
        
        services.AddDbContext<SalesDbContext>(options => 
            options.UseMySql(salesDbConnectionString, ServerVersion.AutoDetect(salesDbConnectionString)));
        
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<SalesDbContext>()
            .AddDefaultTokenProviders();
        
        // Adding configuration for supporting unique emails on IdentityTables
        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@áàâãäÁÀÂÃÄéèêëÉÈÊËíìîïÍÌÎÏóòôõöÓÒÔÕÖúùûüÚÙÛÜçÇñÑýÿÝŸœŒøØåÅæÆß\n";
        });

        return services;
    }
}