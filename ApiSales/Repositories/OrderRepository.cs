using ApiSales.Context;
using ApiSales.Models;
using ApiSales.Repositories.Interfaces;

namespace ApiSales.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApiSalesDbContext context) : base(context)
    {
    }
}