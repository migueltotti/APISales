using Sales.Infrastructure.Context;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(SalesDbContext context) : base(context)
    {
    }
}