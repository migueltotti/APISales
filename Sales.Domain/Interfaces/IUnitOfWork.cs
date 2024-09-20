namespace Sales.Domain.Interfaces;

public interface IUnitOfWork
{
    public IUserRepository UserRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IAffiliateRepository AffiliateRepository { get; }

    Task CommitChanges();
}