using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Repositories;

namespace Sales.Infrastructure.Cache;

public class CacheUserRepository : IUserRepository
{
    private readonly UserRepository _decorated;
    private readonly IMemoryCache _cache;

    public CacheUserRepository(UserRepository decorated, IMemoryCache cache)
    {
        _decorated = decorated;
        _cache = cache;
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

        return await _cache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                entry.Size = 1;
                
                return _decorated.GetByIdAsync(id);
            });
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