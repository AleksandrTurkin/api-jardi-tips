using Microsoft.OpenApi;

namespace JardiTips.WebApi.Swagger;

public static class SwaggerConfig
{
    public static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Jardi Tips API", Version = "v1" });
            options.DocumentFilter<LowercaseSwaggerDocumentFilter>();
        });
    }

    public static IApplicationBuilder UseSwagger(this WebApplication app)
    {
        app.UseSwagger(_ => { });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "v1");
        });

        app.MapGet("/", context =>
        {
            context.Response.Redirect("/swagger");
            return Task.CompletedTask;
        }).AllowAnonymous();
        return app;
    }
}

