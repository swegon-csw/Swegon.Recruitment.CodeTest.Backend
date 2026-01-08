using Swegon.Recruitment.CodeTest.Backend.Api.Middleware;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Extensions;

/// <summary>
/// Extension methods for IApplicationBuilder
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds custom middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        
        return app;
    }
    
    /// <summary>
    /// Configures API documentation
    /// </summary>
    public static IApplicationBuilder UseApiDocumentation(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Swegon Recruitment CodeTest API v1");
                options.RoutePrefix = string.Empty; // Serve swagger at root
            });
        }
        
        return app;
    }
}
