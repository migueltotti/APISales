using System.Text.Json.Serialization;
using Sales.API.ExceptionHandler;
using Sales.CrossCutting.IoC;

namespace Sales.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        
        // Add Services
        builder.Services.AddInfrastructure(builder.Configuration);

/*        // configurando a conexao com o banco de dados MySQL
        string mySqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<SalesDbContext>(options =>
            options.UseMySql(mySqlConnectionString, ServerVersion.AutoDetect(mySqlConnectionString)));
        
        // Add Repoitories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Add DTO Mapping
        builder.Services.AddAutoMapper(typeof(MappingDTO));
*/
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        // Tratamento de execao global usando Middleware, biblioteca IExceptionHandler e ProblemDetails
        // Em conformidade com a RFC 7231 section 6.6.2
        app.UseExceptionHandler();

        app.MapControllers();

        app.Run();
    }
}
