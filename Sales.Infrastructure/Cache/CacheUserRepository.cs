using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Repositories;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Sales.Infrastructure.Cache;

public class CacheUserRepository : IUserRepository
{
    private readonly UserRepository _decorated;
    private readonly IDistributedCache _distributedCache;

    public CacheUserRepository(UserRepository decorated, IDistributedCache distributedCache)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _decorated.GetAllAsync();
    }

    public async Task<User?> GetAsync(Expression<Func<User, bool>> expression)
    {
        return await _decorated.GetAsync(expression);
    }

    public User Create(User entity)
    {
        return _decorated.Create(entity);
    }

    public User Update(User entity)
    {
        return _decorated.Update(entity);
    }

    public User Delete(User entity)
    {
        return _decorated.Delete(entity);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        var key = $"user-{id}";

        var cachedUser = await _distributedCache.GetStringAsync(key);

        User? user;
        if (string.IsNullOrEmpty(cachedUser))
        {
            user = await _decorated.GetByIdAsync(id);

            if (user is null)
            {
                return user;
            }
            
            await _distributedCache.SetStringAsync(
                key,
                JsonSerializer.Serialize(user));
            
            return user;
        }
        
        user = JsonConvert.DeserializeObject<User>(
            cachedUser,
            new JsonSerializerSettings
            {
                ConstructorHandling = 
                    ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });
        
        return user;
    }

    public async Task<IEnumerable<User>> GetUsersOrders()
    {
        return await _decorated.GetUsersOrders();
    }

    public async Task<User?> GetUserOrders(int id)
    {
        return await _decorated.GetUserOrders(id);
    }
}