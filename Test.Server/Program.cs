using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShortenURLService.Data;
using ShortenURLService.Services;
using StackExchange.Redis;
using System.Security.Authentication;

namespace ShortenURLService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string redisConnection = builder.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Connection string 'RedisConnection' not found."); 
            builder.Services.AddDbContext<ShortenURLServiceContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("ShortenURLServiceContext") 
                        ?? throw new InvalidOperationException("Connection string 'ShortenURLServiceContext' not found.")));

            // Configure Redis options
            var redisOptions = ConfigurationOptions.Parse(redisConnection);
            redisOptions.AbortOnConnectFail = false;
            redisOptions.Ssl = true;
            redisOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            redisOptions.ConnectTimeout = 10000;
            redisOptions.SyncTimeout = 10000;

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            try
            {
                Console.WriteLine("Attempting to connect to Redis...");
                var multiplexer = ConnectionMultiplexer.Connect(redisOptions);
                builder.Services.AddSingleton<IConnectionMultiplexer>(sp => multiplexer);
                builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
                Console.WriteLine("✓ Redis connection successful");
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"⚠ Redis connection failed: {ex.Message}");
                Console.WriteLine("Using in-memory cache fallback");
                
                // Add a mock/fallback implementation
                builder.Services.AddSingleton<IRedisCacheService, MockRedisCacheService>();
            }

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<GenerateShortenURL>();
            
            var app = builder.Build();

            // Log startup information
            Console.WriteLine("\n=== URL Shortener Service Starting ===");
            Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
            Console.WriteLine($"Database: {builder.Configuration.GetConnectionString("ShortenURLServiceContext")?.Split(';')[0]}");
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                Console.WriteLine("✓ Swagger UI enabled at /swagger");
            }

            // Use CORS policy
            app.UseCors("AllowAll");

            app.UseHttpsRedirection();      
            app.UseAuthorization();
            app.MapControllers();

            Console.WriteLine("✓ CORS enabled for all origins");
            Console.WriteLine("✓ Controllers mapped");
            Console.WriteLine("\n🚀 Server is ready!");
            Console.WriteLine("Available endpoints:");
            Console.WriteLine("  - HTTPS: https://localhost:7282");
            Console.WriteLine("  - HTTP:  http://localhost:5258");
            Console.WriteLine("  - Swagger: https://localhost:7282/swagger");
            Console.WriteLine("\nPress Ctrl+C to stop the server\n");

            app.Run();
        }
    }
}
