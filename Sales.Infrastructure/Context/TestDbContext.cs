using Microsoft.EntityFrameworkCore;
using Sales.Domain.Models;

namespace Sales.Infrastructure.Context;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    { }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<LineItem> LineItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<WorkDay> WorkDays { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<ShoppingCartProduct> ShoppingCartProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations of ModelsConfigurations files
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly);
    }
}