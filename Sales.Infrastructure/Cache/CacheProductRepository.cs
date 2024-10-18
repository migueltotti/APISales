using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Repositories;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Sales.Infrastructure.Cache;

public class CacheProductRepository : IProductRepository
{
    private readonly ProductRepository _decorator;
    private readonly IDistributedCache _distributedCache;

    public CacheProductRepository(ProductRepository decorator, IDistributedCache distributedCache)
    {
        _decorator = decorator;
        _distributedCache = distributedCache;
    }
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _decorator.GetAllAsync();
    }
    
    public async Task<Product?> GetByIdAsync(int id)
    {
        var key = $"product-{id}";

        var cachedProduct = await _distributedCache.GetStringAsync(key);

        Product? product;
        if (string.IsNullOrEmpty(cachedProduct))
        {
            product = await _decorator.GetByIdAsync(id);

            if (product is null)
            {
                return product;
            }
            
            await _distributedCache.SetStringAsync(
                key,
                JsonSerializer.Serialize(product));
            
            return product;
        }
        
        product = JsonConvert.DeserializeObject<Product>(
            cachedProduct,
            new JsonSerializerSettings
            {
                ConstructorHandling = 
                    ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });
        
        return product;
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