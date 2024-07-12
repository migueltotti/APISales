using ApiSales.Context;
using ApiSales.Models;
using ApiSales.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiSales.Repositories;

public class CategoryRepository(ApiSalesDbContext context) : Repository<Category>(context), ICategoryRepository
{
}