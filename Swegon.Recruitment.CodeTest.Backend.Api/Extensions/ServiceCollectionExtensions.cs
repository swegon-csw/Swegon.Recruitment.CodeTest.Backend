using Swegon.Recruitment.CodeTest.Backend.Api.Config;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Extensions;

/// <summary>
/// Extension methods for IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application services to the service collection
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure settings
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.Configure<CacheConfiguration>(configuration.GetSection("AppSettings:Cache"));
        services.Configure<DatabaseConfig>(configuration.GetSection("Database"));
        
        // Add memory cache
        services.AddMemoryCache();
        
        // Add services
        services.AddScoped<ProductService>();
        services.AddScoped<CalculationService>();
        services.AddScoped<CacheService>();
        services.AddScoped<DataService>();
        services.AddScoped<ValidationService>();
        services.AddScoped<TransformationService>();
        services.AddScoped<ExportService>();
        
        // Add HttpClient
        services.AddHttpClient();
        
        return services;
    }
    
    /// <summary>
    /// Adds API documentation services
    /// </summary>
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "Swegon Recruitment CodeTest API",
                Version = "v1",
                Description = "API for Swegon recruitment code test"
            });
        });
        
        return services;
    }
    
    /// <summary>
    /// Adds CORS policy
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
        
        return services;
    }
}
