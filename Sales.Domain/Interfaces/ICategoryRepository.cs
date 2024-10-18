using Sales.Domain.Models;

namespace Sales.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByIdAsync(int id);
}