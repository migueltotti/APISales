using Sales.Infrastructure.Context;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(SalesDbContext context) : base(context)
    {
    }
}