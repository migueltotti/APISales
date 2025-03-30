namespace Sales.Domain.Interfaces;

public interface IUnitOfWork
{
    public IUserRepository UserRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IAffiliateRepository AffiliateRepository { get; }
    public IShoppingCartRepository ShoppingCartRepository { get; }
    public IWorkDayRepository WorkDayRepository { get; }

    Task CommitChanges();
}