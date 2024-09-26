using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Sales.Infrastructure.Identity;

namespace Sales.Infrastructure.Context;

public class UsersDataDbContext : IdentityDbContext<ApplicationUser>
{
    public UsersDataDbContext(DbContextOptions<UsersDataDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
    //dotnet ef migrations add "IdetityTables-initialmigration" --context UsersDataDbContext --output-dir Migrations/PostGresMigrations -s ..\Sales.API\Sales.API.csproj

}