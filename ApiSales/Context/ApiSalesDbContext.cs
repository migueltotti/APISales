using ApiSales.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiSales.Context;

public class ApiSalesDbContext : DbContext
{
    public ApiSalesDbContext(DbContextOptions<ApiSalesDbContext> options) : base(options)
    { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Employee> Employees { get; set; }
}