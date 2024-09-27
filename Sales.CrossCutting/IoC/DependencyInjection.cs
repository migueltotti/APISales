using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.DTOs.AffiliateDTO;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.DTOs.TokenDTO;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.Interfaces;
using Sales.Application.Mapping;
using Sales.Application.Services;
using Sales.Application.Strategy;
using Sales.Application.Strategy.Factory;
using Sales.Application.Strategy.FilterImplementation.CategoryStrategy;
using Sales.Application.Strategy.FilterImplementation.OrderStrategy;
using Sales.Application.Strategy.FilterImplementation.ProductStrategy;
using Sales.Application.Strategy.FilterImplementation.UserStrategy;
using Sales.Application.Validations;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Repositories;
using Sales.Infrastructure.Context;
using Sales.Infrastructure.Identity;

namespace Sales.CrossCutting.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var mySqlConnectionString = configuration.GetConnectionString("DefaultMySqlConnection") ?? throw new NullReferenceException("MySQL connection string is null");
        var postGreSqlConnectionString = configuration.GetConnectionString("DefaultPostGresConnection") ?? throw new NullReferenceException("PostGreSQL connection string is null");
        
        // Add DataBase connection
        services.AddDbContext<SalesDbContext>(options => 
                options.UseMySql(mySqlConnectionString,
                new MySqlServerVersion(new Version(8, 0, 38))));
        
        // Add Database UsersData - PostGres connection
        services.AddEntityFrameworkNpgsql().AddDbContext<UsersDataDbContext>(options =>
            options.UseNpgsql(postGreSqlConnectionString));
        
        // Identity tables configuration
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<UsersDataDbContext>()
            .AddDefaultTokenProviders();
        
        // Add Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IAffiliateRepository, AffiliateRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Add Services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IAffiliateService, AffiliateService>();
        services.AddScoped<ITokenService, TokenService>();
        
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
        
        // Add Factory Pattern
        services.AddScoped<ICategoryFilterFactory, CategoryFilterFactory>();
        services.AddScoped<IProductFilterFactory, ProductFilterFactory>();
        services.AddScoped<IUserFilterFactory, UserFilterFactory>();
        services.AddScoped<IOrderFilterFactory, OrderFilterFactory>();
        
        // Add FluentValidation
        services.AddScoped<IValidator<CategoryDTOInput>, CategoryValidator>();
        services.AddScoped<IValidator<ProductDTOInput>, ProductValidator>();
        services.AddScoped<IValidator<UserDTOInput>, UserValidator>();
        services.AddScoped<IValidator<OrderDTOInput>, OrderValidator>();
        services.AddScoped<IValidator<AffiliateDTOInput>, AffiliateValidator>();
        services.AddScoped<IValidator<LoginModel>, LoginValidator>();
        services.AddScoped<IValidator<RegisterModel>, RegisterValidator>();
        
        // Add DTO Mapping
        services.AddAutoMapper(typeof(MappingDTO));
        
        return services;
    }
}