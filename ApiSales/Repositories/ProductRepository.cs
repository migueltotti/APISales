using ApiSales.Context;
using ApiSales.Models;
using ApiSales.Repositories.Interfaces;

namespace ApiSales.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApiSalesDbContext context) : base(context)
    {
    }
}