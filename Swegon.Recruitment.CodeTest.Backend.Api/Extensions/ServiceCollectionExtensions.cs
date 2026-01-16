using Swegon.Recruitment.CodeTest.Backend.Api.Services;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<JsonDataService>();
        services.AddSingleton<ProductService>();
        services.AddSingleton<CalculationService>();
        return services;
    }

    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new()
                {
                    Title = "Swegon Recruitment CodeTest API",
                    Version = "v1",
                    Description = "API for Swegon recruitment code test",
                }
            );
        });

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "DefaultPolicy",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
            );
        });

        return services;
    }
}
