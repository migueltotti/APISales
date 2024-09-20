using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.DTOs.AffiliateDTO;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
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

namespace Sales.CrossCutting.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DataBase connection
        services.AddDbContext<SalesDbContext>(options => 
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 38))));
        
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
        
        // Add Strategy Pattern
        services.AddScoped<ICategoryFilterStrategy, CategoryNameFilter>();
        
        services.AddScoped<IProductFilterStrategy, ProductNameFilter>();
        services.AddScoped<IProductFilterStrategy, ProductValueFilter>();
        services.AddScoped<IProductFilterStrategy, ProductTypeValueFilter>();
        
        services.AddScoped<IUserFilterStrategy, UserNameFilter>();
        services.AddScoped<IUserFilterStrategy, UserRoleFilter>();
        
        services.AddScoped<IOrderFilterStrategy, OrderDateFilter>();
        services.AddScoped<IOrderFilterStrategy, OrderValueFilter>();
        
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
        
        // Add DTO Mapping
        services.AddAutoMapper(typeof(MappingDTO));
        
        return services;
    }
}