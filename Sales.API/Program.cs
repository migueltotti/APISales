using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NuGet.Packaging.Signing;
using Sales.API.ExceptionHandler;
using Sales.API.Filter;
using Sales.CrossCutting.IoC;
using Serilog;

namespace Sales.API;

public class Program
{
    public static void Main(string[] args)
    {
        // Load env_variables values from .env file
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            DotEnv.Load();
        }
        
        var builder = WebApplication.CreateBuilder(args);
        

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<LoggingRequestsFilter>();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        builder.Services.AddCors( options =>
        {
            options.AddPolicy("CorsEx",
                police =>
                {
                    police.WithOrigins("https://apirequest.io")
                        .WithMethods("GET");
                });
            options.AddPolicy("EnableCors", police =>
            {
                police.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination").Build();
            });
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(httpcontext.User.Identity?.Name ??
                                                         httpcontext.Request.Headers.Host.ToString(),
                    partition => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 10,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(5),
                        TokensPerPeriod = 8,
                        AutoReplenishment = true,
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    }));
        });
        
        // Add Services
        builder.Services.AddInfrastructure(builder.Configuration);
        
        // Add Authentication and Authorization with JwtBearerToken
        // IMPORTANT!!!
        // AddInfrastructure() must come before AddAuthentication
        // `cause the service AddIdentity<>()... has to be set before the AddAuthentication
        var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY")
                        ?? throw new ArgumentNullException("Invalid SecretKey!");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = builder.Configuration["JWT:ValidAudience"],
                ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey))
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("AdminEmployeeOnly", policy => policy.RequireRole("Admin", "Employee"));
            options.AddPolicy("AllowAnyUser", policy => policy.RequireRole("Admin", "Employee", "Customer"));
        });
        

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sales", Version = "v1" });

            //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            
            //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Bearer JWT "
            });

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[]{ }
                }
            });
        });
        
        builder.Host.UseSerilog((context, configuration) => 
            configuration.ReadFrom.Configuration(context.Configuration));

        /*builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(443, listenOptions =>
            {
                listenOptions.UseHttps("/https/cert.pfx", Environment.GetEnvironmentVariable("CERT_PASSWD"));
            });
        });*/

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseRateLimiter();
        app.UseCors("EnableCors");

        app.UseAuthentication();
        app.UseAuthorization();
        
        // Tratamento de execao global usando Middleware, biblioteca IExceptionHandler e ProblemDetails
        // Em conformidade com a RFC 7231 section 6.6.2
        app.UseExceptionHandler();

        app.MapControllers();

        app.Run();
    }
}
