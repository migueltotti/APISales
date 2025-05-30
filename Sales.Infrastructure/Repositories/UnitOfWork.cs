using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Sales.Infrastructure.Context;
using Sales.Domain.Interfaces;
using Sales.Infrastructure.Cache;

namespace Sales.Infrastructure.Repositories;

public class UnitOfWork(SalesDbContext context, IDistributedCache distributedCache) : IUnitOfWork
{
    private IUserRepository? _userRepository;
    private ICategoryRepository? _categoryRepository;
    private IOrderRepository? _orderRepository;
    private IProductRepository? _productRepository;
    private IAffiliateRepository? _affiliateRepository;
    private IShoppingCartRepository? _shoppingCartRepository;
    private IWorkDayRepository? _workDayRepository;

    public IUserRepository UserRepository
    {
        get
        {
            return _userRepository = _userRepository ?? new CacheUserRepository(
                new UserRepository(context), distributedCache);
        }
    }

    public ICategoryRepository CategoryRepository
    {
        get
        {
            return _categoryRepository = _categoryRepository ?? new CacheCategoryRepository(
                new CategoryRepository(context), distributedCache);
        }
    }
    
    public IOrderRepository OrderRepository
    {
        get
        {
            return _orderRepository = _orderRepository ?? new CacheOrderRepository(
                new OrderRepository(context), distributedCache);
        }
    }
    
    public IProductRepository ProductRepository
    {
        get
        {
            return _productRepository = _productRepository ?? new CacheProductRepository(
                new ProductRepository(context), distributedCache);
        }
    }
    
    public IAffiliateRepository AffiliateRepository
    {
        get
        {
            return _affiliateRepository = _affiliateRepository ?? new CacheAffiliateRepository(
                new AffiliateRepository(context), distributedCache);
        }
    }
    
    public IShoppingCartRepository ShoppingCartRepository
    {
        get
        {
            return _shoppingCartRepository = _shoppingCartRepository ?? new ShoppingCartRepository(context);
        }
    }

    public IWorkDayRepository WorkDayRepository
    {
        get
        {
            return _workDayRepository = _workDayRepository ?? new WorkDayRepository(context);
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