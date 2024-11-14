using Sales.Domain.Models;

namespace Sales.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdAsync(int id);
    Task UpdateCacheAsync(Product updatedProduct);
}