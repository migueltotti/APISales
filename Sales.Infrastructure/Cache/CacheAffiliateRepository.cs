using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Repositories;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Sales.Infrastructure.Cache;

public class CacheAffiliateRepository : IAffiliateRepository
{
    private readonly AffiliateRepository _decorated;
    private readonly IDistributedCache _distributedCache;

    public CacheAffiliateRepository(AffiliateRepository decorated, IDistributedCache distributedCache)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
    }

    public Task<IEnumerable<Affiliate>> GetAllAsync()
    {
        return _decorated.GetAllAsync();
    }
    
    public async Task<Affiliate?> GetByIdAsync(int id)
    {
        var key = $"affiliate-{id}";

        var cachedAffiliate = await _distributedCache.GetStringAsync(key);

        Affiliate? affiliate;
        if (string.IsNullOrEmpty(cachedAffiliate))
        {
            affiliate = await _decorated.GetByIdAsync(id);

            if (affiliate is null)
            {
                return affiliate;
            }
            
            await _distributedCache.SetStringAsync(
                key,
                JsonSerializer.Serialize(affiliate));
            
            return affiliate;
        }
        
        affiliate = JsonConvert.DeserializeObject<Affiliate>(
            cachedAffiliate,
            new JsonSerializerSettings
            {
                ConstructorHandling = 
                    ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });
        
        return affiliate;
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