using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Repositories;

namespace Sales.Infrastructure.Cache;

public class CacheProductRepository : IProductRepository
{
    private readonly ProductRepository _decorator;
    private readonly IMemoryCache _cache;

    public CacheProductRepository(ProductRepository decorator, IMemoryCache cache)
    {
        _decorator = decorator;
        _cache = cache;
    }
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _decorator.GetAllAsync();
    }
    
    public async Task<Product?> GetByIdAsync(int id)
    {
        var key = $"product-{id}";

        return await _cache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                entry.Size = 1;
                
                return _decorator.GetByIdAsync(id);
            });
    }

    public async Task<Product?> GetAsync(Expression<Func<Product, bool>> expression)
    {
        return await _decorator.GetAsync(expression);
    }

    public Product Create(Product entity)
    {
        return _decorator.Create(entity);
    }

    public Product Update(Product entity)
    {
        return _decorator.Update(entity);
    }

    public Product Delete(Product entity)
    {
        return _decorator.Delete(entity);
    }
}