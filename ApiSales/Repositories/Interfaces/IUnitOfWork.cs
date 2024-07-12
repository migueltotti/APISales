namespace ApiSales.Repositories.Interfaces;

public interface IUnitOfWork
{
    public IEmployeeRepository EmployeeRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public IProductRepository ProductRepository { get; }

    Task CommitChanges();
}