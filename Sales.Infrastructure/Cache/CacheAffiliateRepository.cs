using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Repositories;

namespace Sales.Infrastructure.Cache;

public class CacheAffiliateRepository : IAffiliateRepository
{
    private readonly AffiliateRepository _decorated;
    private readonly IMemoryCache _cache;

    public CacheAffiliateRepository(AffiliateRepository decorated, IMemoryCache cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    public Task<IEnumerable<Affiliate>> GetAllAsync()
    {
        return _decorated.GetAllAsync();
    }
    
    public async Task<Affiliate?> GetByIdAsync(int id)
    {
        var key = $"affiliate-{id}";

        return await _cache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                entry.Size = 1;
                
                return _decorated.GetByIdAsync(id);
            });
    }

    public Task<Affiliate?> GetAsync(Expression<Func<Affiliate, bool>> expression)
    {
        return _decorated.GetAsync(expression);
    }

    public Affiliate Create(Affiliate entity)
    {
        return _decorated.Create(entity);
    }

    public Affiliate Update(Affiliate entity)
    {
        return _decorated.Update(entity);
    }

    public Affiliate Delete(Affiliate entity)
    {
        return _decorated.Delete(entity);
    }
}