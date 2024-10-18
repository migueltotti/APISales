using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Repositories;

namespace Sales.Infrastructure.Cache;

public class CacheCategoryRepository : ICategoryRepository
{
    private readonly CategoryRepository _decorator;
    private readonly IMemoryCache _cache;

    public CacheCategoryRepository(CategoryRepository decorator, IMemoryCache cache)
    {
        _decorator = decorator;
        _cache = cache;
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

        return await _cache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                entry.Size = 1;
                
                return _decorator.GetByIdAsync(id);
            });
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