using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using NuGet.Packaging.Signing;
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

        builder.Services.AddCors(/*options =>
        {
            options.AddPolicy("CorsEx",
                police =>
                {
                    police.WithOrigins("https://apirequest.io")
                        .WithMethods("GET");
                });
        }*/);

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(httpcontext.User.Identity?.Name ??
                                                         httpcontext.Request.Headers.Host.ToString(),
                    partition => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 3,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(5),
                        TokensPerPeriod = 2,
                        AutoReplenishment = true,
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    }));
        });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        
        // Add Services
        builder.Services.AddInfrastructure(builder.Configuration);
        
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

        app.UseRateLimiter();
        app.UseCors("CorsEx");

        app.UseAuthorization();
        
        // Tratamento de execao global usando Middleware, biblioteca IExceptionHandler e ProblemDetails
        // Em conformidade com a RFC 7231 section 6.6.2
        app.UseExceptionHandler();

        app.MapControllers();

        app.Run();
    }
}
