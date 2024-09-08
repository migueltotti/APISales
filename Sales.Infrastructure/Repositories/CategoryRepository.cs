using Sales.Infrastructure.Context;
using Sales.Domain.Models;
using Sales.Domain.Interfaces;

namespace Sales.Infrastructure.Repositories;

public class CategoryRepository(SalesDbContext context) : Repository<Category>(context), ICategoryRepository
{
}