using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Repositories;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Sales.Infrastructure.Cache;

public class CacheCategoryRepository : ICategoryRepository
{
    private readonly CategoryRepository _decorator;
    private readonly IDistributedCache _distributedCache;

    public CacheCategoryRepository(CategoryRepository decorator, IDistributedCache distributedCachecache)
    {
        _decorator = decorator;
        _distributedCache = distributedCachecache;
    }
   
    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _decorator.GetAllAsync();
    }

    public async Task<Category?> GetAsync(Expression<Func<Category, bool>> expression)
    {
        return await _decorator.GetAsync(expression);
    }
    
    public async Task<Category?> GetByIdAsync(int id)
    {
        var key = $"category-{id}";

        var cachedCategory = await _distributedCache.GetStringAsync(key);

        Category? category;
        if (string.IsNullOrEmpty(cachedCategory))
        {
            category = await _decorator.GetByIdAsync(id);

            if (category is null)
            {
                return category;
            }
            
            await _distributedCache.SetStringAsync(
                key,
                JsonSerializer.Serialize(category));
            
            return category;
        }
        
        category = JsonConvert.DeserializeObject<Category>(
            cachedCategory,
            new JsonSerializerSettings
            {
                ConstructorHandling = 
                    ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });
        
        return category;
    }

    public Category Create(Category entity)
    {
        return _decorator.Create(entity);
    }

    public Category Update(Category entity)
    {
        return _decorator.Update(entity);
    }

    public Category Delete(Category entity)
    {
        return _decorator.Delete(entity);
    }
}