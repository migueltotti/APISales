using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Repositories;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Sales.Infrastructure.Cache;

public class CacheWorkDayRepository : IWorkDayRepository
{
    private readonly WorkDayRepository _decorator;
    private readonly IDistributedCache _cache;

    public CacheWorkDayRepository(WorkDayRepository decorator, IDistributedCache cache)
    {
        _cache = cache;
        _decorator = decorator;
    }

    public Task<IEnumerable<WorkDay>> GetAllAsync()
    {
        return _decorator.GetAllAsync();
    }
    
    public async Task<WorkDay?> GetByIdAsync(int id)
    {
        var key = $"workDay-{id}";

        var cachedWorkDay = await _cache.GetStringAsync(key);

        WorkDay? workDay;
        if (string.IsNullOrEmpty(cachedWorkDay))
        {
            workDay = await _decorator.GetByIdAsync(id);

            if (workDay is null)
            {
                return workDay;
            }
            
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpirationTime.OneDay
            };
            
            await _cache.SetStringAsync(
                key,
                JsonSerializer.Serialize(workDay),
                cacheOptions);
            
            return workDay;
        }
        
        workDay = JsonConvert.DeserializeObject<WorkDay>(
            cachedWorkDay,
            new JsonSerializerSettings
            {
                ConstructorHandling = 
                    ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });
        
        return workDay;
    }

    public Task<WorkDay?> GetAsync(Expression<Func<WorkDay, bool>> expression)
    {
        return _decorator.GetAsync(expression);
    }

    public WorkDay Create(WorkDay entity)
    {
        return _decorator.Create(entity);
    }

    public WorkDay Update(WorkDay entity)
    {
        return _decorator.Update(entity);
    }

    public WorkDay Delete(WorkDay entity)
    {
        return _decorator.Delete(entity);
    }
}