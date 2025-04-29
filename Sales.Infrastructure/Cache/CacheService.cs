using Microsoft.Extensions.Caching.Distributed;
using Sales.Domain.Interfaces;

namespace Sales.Infrastructure.Cache;

public class CacheService : ICacheService
{
    private static IDistributedCache _distributedCache;

    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }
}