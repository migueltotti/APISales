using Microsoft.EntityFrameworkCore;
using Sales.Domain.Models;

namespace Sales.Infrastructure.Context;

public class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options)
    { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<ShoppingCartProduct> ShoppingCartProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations of ModelsConfigurations files
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly);
    }
    
    //dotnet ef migrations add "IdetityTables-initialmigration" --context UsersDataDbContext --output-dir Migrations/PostGresMigrations -p .\Sales.Infrastructure\Sales.Infrastructure.csproj -s .\Sales.API\Sales.API.csproj
    
    //dotnet ef database update --context SalesDbContext -s ..\Sales.API\Sales.API.csproj -v
    //dotnet ef migrations add "" -s ..\Sales.API\Sales.API.csproj
    // para executar ambos os comandos, precisamos estar na pasta Sales.Infrastructure no terminal
}