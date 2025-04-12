using System.Collections.Immutable;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.DTOs.AffiliateDTO;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.DTOs.TokenDTO;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.DTOs.WorkDayDTO;
using Sales.Application.Interfaces;
using Sales.Application.Mapping;
using Sales.Application.MassTransit.GenerateReport;
using Sales.Application.Services;
using Sales.Application.Strategy;
using Sales.Application.Strategy.Factory;
using Sales.Application.Strategy.FilterImplementation.CategoryStrategy;
using Sales.Application.Strategy.FilterImplementation.OrderStrategy;
using Sales.Application.Strategy.FilterImplementation.ProductStrategy;
using Sales.Application.Strategy.FilterImplementation.UserStrategy;
using Sales.Application.Strategy.GenerateReport;
using Sales.Application.Validations;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Cache;
using Sales.Infrastructure.Repositories;
using Sales.Infrastructure.Context;
using Sales.Infrastructure.Identity;
using Sales.Infrastructure.MassTransit;

namespace Sales.CrossCutting.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var mySqlConnectionString = configuration.GetConnectionString("DefaultMySqlConnection") ?? throw new NullReferenceException("MySQL connection string is null");
        var postGreSqlConnectionString = configuration.GetConnectionString("DefaultPostGresConnection") ?? throw new NullReferenceException("PostGreSQL connection string is null");
        var testConnectionString = configuration.GetConnectionString("TestDbMySqlConnection") ?? throw new NullReferenceException("Test connection string is null");
        var rabbitConnectionString = configuration.GetConnectionString("RabbitMQ") ?? throw new NullReferenceException("Test connection string is null");
        
        // Add DataBase connection
        services.AddDbContext<SalesDbContext>(options => 
                options.UseMySql(mySqlConnectionString,
                new MySqlServerVersion(new Version(8, 0, 38))));
        
        // Add Database UsersData - PostGres connection
        services.AddEntityFrameworkNpgsql().AddDbContext<UsersDataDbContext>(options =>
            options.UseNpgsql(postGreSqlConnectionString));
        
        // Test DataBase
        services.AddDbContext<TestDbContext>(options => 
            options.UseMySql(testConnectionString,
                new MySqlServerVersion(new Version(8, 0, 38))));
        
        // Identity tables configuration
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<UsersDataDbContext>()
            .AddDefaultTokenProviders();
        
        // Adding configuration for supporting unique emails on IdentityTables
        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
        });

        // Configure Redis
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            string connection = configuration.GetConnectionString("Redis")!;

            redisOptions.Configuration = connection;
        });
        
        // Add RabbitMq to MassTransit
        services.AddRabbitMQ(configuration);
        
        // Add FluentEmail
        services.AddFluentEmail(configuration["EmailSettings:FromEmail"], configuration["EmailSettings:FromName"])
            .AddSmtpSender(configuration["EmailSettings:Host"], configuration.GetValue<int>("EmailSettings:Port"));
        
        // Add Repositories
        services.AddScoped<UserRepository>();
        services.AddScoped<IUserRepository, CacheUserRepository>();
        
        services.AddScoped<CategoryRepository>();
        services.AddScoped<ICategoryRepository, CacheCategoryRepository>();
        
        services.AddScoped<OrderRepository>();
        services.AddScoped<IOrderRepository, CacheOrderRepository>();
        
        services.AddScoped<ProductRepository>();
        services.AddScoped<IProductRepository, CacheProductRepository>();
        
        services.AddScoped<AffiliateRepository>();
        services.AddScoped<IAffiliateRepository, CacheAffiliateRepository>();
        
        services.AddScoped<WorkDayRepository>();
        services.AddScoped<IWorkDayRepository, CacheWorkDayRepository>();
        
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Add Services
        services.AddTransient<ISendBusMessage, SendBusMessage>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IAffiliateService, AffiliateService>();
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<IWorkDayService, WorkDayService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<ISendEmailService, SendEmailService>();
        
        // Add Strategy Pattern
        services.AddScoped<ICategoryFilterStrategy, CategoryNameFilter>();
        
        services.AddScoped<IProductFilterStrategy, ProductNameFilter>();
        services.AddScoped<IProductFilterStrategy, ProductValueFilter>();
        services.AddScoped<IProductFilterStrategy, ProductTypeValueFilter>();
        
        services.AddScoped<IUserFilterStrategy, UserNameFilter>();
        services.AddScoped<IUserFilterStrategy, UserRoleFilter>();
        
        services.AddScoped<IOrderFilterStrategy, OrderDateFilter>();
        services.AddScoped<IOrderFilterStrategy, OrderValueFilter>();
        services.AddScoped<IOrderFilterStrategy, OrderStatusFilter>();
        
        services.AddScoped<IGenerateReport, GeneratePOSEXCELReport>();
        services.AddScoped<IGenerateReport, GeneratePOSPDFReport>();
        
        // Add Factory Pattern
        services.AddScoped<ICategoryFilterFactory, CategoryFilterFactory>();
        services.AddScoped<IProductFilterFactory, ProductFilterFactory>();
        services.AddScoped<IUserFilterFactory, UserFilterFactory>();
        services.AddScoped<IOrderFilterFactory, OrderFilterFactory>();
        services.AddScoped<IGenerateReportFactory, GenerateReportFactory>();
        
        // Add FluentValidation
        services.AddScoped<IValidator<CategoryDTOInput>, CategoryValidator>();
        services.AddScoped<IValidator<ProductDTOInput>, ProductValidator>();
        services.AddScoped<IValidator<UserDTOInput>, UserValidator>();
        services.AddScoped<IValidator<UserUpdateDTO>, UserUpdateValidator>();
        services.AddScoped<IValidator<OrderDTOInput>, OrderValidator>();
        services.AddScoped<IValidator<WorkDayDTOInput>, WorkDayValidator>();
        services.AddScoped<IValidator<AffiliateDTOInput>, AffiliateValidator>();
        services.AddScoped<IValidator<LoginModel>, LoginValidator>();
        services.AddScoped<IValidator<RegisterModel>, RegisterValidator>();
        
        // Add MassTransit Bus
        services.AddTransient<ISendBusMessage, SendBusMessage>();
        
        // Add DTO Mapping
        services.AddAutoMapper(typeof(MappingDTO));
        
        return services;
    }
}