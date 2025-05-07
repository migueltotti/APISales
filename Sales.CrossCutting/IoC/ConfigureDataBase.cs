using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Infrastructure.Context;
using Sales.Infrastructure.Identity;

namespace Sales.CrossCutting.IoC;

public static class ConfigureDataBase
{
    public static IServiceCollection AddDataBase(this IServiceCollection services, IConfiguration configuration)
    {
        //var mySqlConnectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING") ?? throw new NullReferenceException("MySQL connection string is null");
        var postGreSqlConnectionString = configuration.GetConnectionString("DefaultPostGresConnection") ?? throw new NullReferenceException("PostGreSQL connection string is null");
        var testConnectionString = configuration.GetConnectionString("TestDbMySqlConnection") ?? throw new NullReferenceException("Test connection string is null");
        var mergeDbConnectionString = Environment.GetEnvironmentVariable("mergeDbConnectionString") ?? throw new NullReferenceException("Merge Db connection string is null");
        
        // Add DataBase connection
        /*services.AddDbContext<SalesDbContext>(options => 
            options.UseMySql(mySqlConnectionString,
                new MySqlServerVersion(new Version(8, 0, 38))));
        
        // Add Database UsersData - PostGreSql connection
        services.AddEntityFrameworkNpgsql().AddDbContext<UsersDataDbContext>(options =>
            options.UseNpgsql(postGreSqlConnectionString));
        
        // Test DataBase
        services.AddDbContext<TestDbContext>(options => 
            options.UseMySql(testConnectionString,
                new MySqlServerVersion(new Version(8, 0, 38))));
        
        // Identity tables configuration
        services.AddIdentity<ApplicationUser, IdentityRole>()
             .AddEntityFrameworkStores<UsersDataDbContext>()
             .AddDefaultTokenProviders();*/
        
        // TODO: Change repositories database context reference to MergeDb
        // TODO: Comment MySql, Test and UserData databases configuration
        
        //Merge DataBase
        services.AddDbContext<MergeDbContext>(options => 
            options.UseMySql(mergeDbConnectionString,
                new MySqlServerVersion(new Version(8, 0, 38))));
        
        // Merge Identity tables configuration
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<MergeDbContext>()
            .AddDefaultTokenProviders();
        
        // Adding configuration for supporting unique emails on IdentityTables
        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
        });

        return services;
    }
}