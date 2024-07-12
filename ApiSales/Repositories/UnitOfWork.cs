using ApiSales.Context;
using ApiSales.Repositories.Interfaces;

namespace ApiSales.Repositories;

public class UnitOfWork(ApiSalesDbContext context) : IUnitOfWork
{
    private IEmployeeRepository? _employeeRepository;
    private ICategoryRepository? _categoryRepository;
    private IOrderRepository? _orderRepository;
    private IProductRepository? _productRepository;

    public IEmployeeRepository EmployeeRepository
    {
        get
        {
            return _employeeRepository = _employeeRepository ?? new EmployeeRepository(context);
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