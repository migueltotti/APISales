using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
        
        var secretKey = builder.Configuration["JWT:SecretKey"]
                    ?? throw new ArgumentNullException("Invalid SecretKey!");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = builder.Configuration["JWT:ValidAudience"],
                ValidIssuer = builder.Configuration["JWT:ValidAIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
            options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
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

        app.UseAuthentication();
        app.UseAuthorization();
        
        // Tratamento de execao global usando Middleware, biblioteca IExceptionHandler e ProblemDetails
        // Em conformidade com a RFC 7231 section 6.6.2
        app.UseExceptionHandler();

        app.MapControllers();

        app.Run();
    }
}
