using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Domain.Models.Enums;
using Sales.Infrastructure.Repositories;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Sales.Infrastructure.Cache;

public class CacheOrderRepository : IOrderRepository
{
    private readonly OrderRepository _decorator;
    private readonly IDistributedCache _distributedCache;

    public CacheOrderRepository(OrderRepository decorator, IDistributedCache distributedCache)
    {
        _decorator = decorator;
        _distributedCache = distributedCache;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _decorator.GetAllAsync();
    }

    public async Task<Order?> GetAsync(Expression<Func<Order, bool>> expression)
    {
        return await _decorator.GetAsync(expression);
    }

    public Order Create(Order entity)
    {
        return _decorator.Create(entity);
    }

    public Order Update(Order entity)
    {
        return _decorator.Update(entity);
    }

    public Order Delete(Order entity)
    {
        return _decorator.Delete(entity);
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        var key = $"order-{id}";

        var cachedOrder = await _distributedCache.GetStringAsync(key);

        Order? order;
        if (string.IsNullOrEmpty(cachedOrder))
        {
            order = await _decorator.GetByIdAsync(id);

            if (order is null)
            {
                return order;
            }

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpirationTime.OneDay
            };

            await _distributedCache.SetStringAsync(
                key,
                JsonSerializer.Serialize(order),
                cacheOptions);
            
            return order;
        }
        
        order = JsonConvert.DeserializeObject<Order>(
            cachedOrder,
            new JsonSerializerSettings
            {
                ConstructorHandling = 
                    ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });
        
        return order;
    }

    public Task<IEnumerable<Order>> GetAllOrdersWithProductsByTodayDate(Status orderStatus)
    {
        return _decorator.GetAllOrdersWithProductsByTodayDate(orderStatus);
    }

    public async Task<IEnumerable<Order>> GetOrdersByProduct(string productName)
    {
        return await _decorator.GetOrdersByProduct(productName);
    }

    public Task<IEnumerable<Order>> GetOrdersWithProductsByUserId(int userId)
    {
        return _decorator.GetOrdersWithProductsByUserId(userId);
    }

    public async Task<IEnumerable<Product>> GetProductsByDate(DateTime minDate, DateTime maxDate)
    {
        return await _decorator.GetProductsByDate(minDate, maxDate);
    }

    public async Task<IEnumerable<Order>> GetOrdersByAffiliateId(int affiliateId)
    {
        return await _decorator.GetOrdersByAffiliateId(affiliateId);
    }

    public Task<Order?> GetOrderWithProductsByOrderId(int orderId)
    {
        return _decorator.GetOrderWithProductsByOrderId(orderId);
    }

    public async Task<int> AddProduct(int orderId, int productId, decimal amount)
    {
        return await _decorator.AddProduct(orderId, productId, amount);
    }

    public async Task<IEnumerable<Product>> GetProducts(int orderId)
    {
        return await _decorator.GetProducts(orderId);
    }

    public Task<LineItem?> GetLineItemByOrderIdAndProductId(int orderId, int productId)
    {
        return _decorator.GetLineItemByOrderIdAndProductId(orderId, productId);
    }

    public Task<IEnumerable<LineItem>?> GetLineItemsByOrderIdAndUserId(List<int> orderIds, int userId)
    {
        return _decorator.GetLineItemsByOrderIdAndUserId(orderIds, userId);
    }

    public async Task<int> RemoveProduct(int orderId, int productId)
    {
        return await _decorator.RemoveProduct(orderId, productId);
    }
}