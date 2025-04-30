using Sales.Infrastructure.Context;
using Sales.Domain.Models;
using Sales.Domain.Interfaces;

namespace Sales.Infrastructure.Repositories;

public class CategoryRepository(MergeDbContext context) : Repository<Category>(context), ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(int id)
    {
        return await GetAsync(c => c.CategoryId == id);
    }
}