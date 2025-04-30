using Sales.Infrastructure.Context;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(MergeDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await GetAsync(p => p.ProductId == id);
    }

    public Task UpdateCacheAsync(Product updatedProduct)
    {
        throw new NotImplementedException();
    }
}