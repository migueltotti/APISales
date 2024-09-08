using Sales.Infrastructure.Context;
using Sales.Domain.Interfaces;

namespace Sales.Infrastructure.Repositories;

public class UnitOfWork(SalesDbContext context) : IUnitOfWork
{
    private IUserRepository? _userRepository;
    private ICategoryRepository? _categoryRepository;
    private IOrderRepository? _orderRepository;
    private IProductRepository? _productRepository;

    public IUserRepository UserRepository
    {
        get
        {
            return _userRepository = _userRepository ?? new UserRepository(context);
        }
    }

    public ICategoryRepository CategoryRepository
    {
        get
        {
            return _categoryRepository = _categoryRepository ?? new CategoryRepository(context);
        }
    }
    
    public IOrderRepository OrderRepository
    {
        get
        {
            return _orderRepository = _orderRepository ?? new OrderRepository(context);
        }
    }
    
    public IProductRepository ProductRepository
    {
        get
        {
            return _productRepository = _productRepository ?? new ProductRepository(context);
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