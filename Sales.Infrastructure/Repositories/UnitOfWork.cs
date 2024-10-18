using Microsoft.Extensions.Caching.Memory;
using Sales.Infrastructure.Context;
using Sales.Domain.Interfaces;
using Sales.Infrastructure.Cache;

namespace Sales.Infrastructure.Repositories;

public class UnitOfWork(SalesDbContext context, IMemoryCache cache) : IUnitOfWork
{
    private IUserRepository? _userRepository;
    private ICategoryRepository? _categoryRepository;
    private IOrderRepository? _orderRepository;
    private IProductRepository? _productRepository;
    private IAffiliateRepository? _affiliateRepository;

    public IUserRepository UserRepository
    {
        get
        {
            return _userRepository = _userRepository ?? new CacheUserRepository(
                new UserRepository(context), cache);
        }
    }

    public ICategoryRepository CategoryRepository
    {
        get
        {
            return _categoryRepository = _categoryRepository ?? new CacheCategoryRepository(
                new CategoryRepository(context), cache);
        }
    }
    
    public IOrderRepository OrderRepository
    {
        get
        {
            return _orderRepository = _orderRepository ?? new CacheOrderRepository(
                new OrderRepository(context), cache);
        }
    }
    
    public IProductRepository ProductRepository
    {
        get
        {
            return _productRepository = _productRepository ?? new CacheProductRepository(
                new ProductRepository(context), cache);
        }
    }
    
    public IAffiliateRepository AffiliateRepository
    {
        get
        {
            return _affiliateRepository = _affiliateRepository ?? new CacheAffiliateRepository(
                new AffiliateRepository(context), cache);
        }
    }

    public async Task CommitChanges()
    {
        await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        context.Dispose();
    }
}