using Swegon.Recruitment.CodeTest.Backend.Api.Middleware;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();

        return app;
    }

    public static IApplicationBuilder UseApiDocumentation(
        this IApplicationBuilder app,
        IWebHostEnvironment env
    )
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "Swegon Recruitment CodeTest API v1"
                );
                options.RoutePrefix = string.Empty;
            });
        }

        return app;
    }
}
