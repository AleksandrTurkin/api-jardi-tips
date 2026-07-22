using JardiTips.WebApi.Endpoints.Base;
using JardiTips.WebApi.ExceptionHandlers;
using JardiTips.WebApi.Swagger;
using System.Reflection;

namespace JardiTips.WebApi;

public static class AppConfiguration
{
    public static void Add(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.AddServiceDefaults();
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        var entryAssembly = Assembly.GetEntryAssembly() ?? throw new Exception("Failed to get the entry assembly");
        builder.Services.AddEndpoints(entryAssembly);
        builder.AddServices();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.AddSwagger();
        }
    }
        
    public static void Use(this WebApplication app)
    {
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi().AllowAnonymous();
            app.UseSwagger();
        }

        app.UseCors("AllowAll");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        var group = app.MapGroup("api");
        app.MapEndpoints(group);
    }
}

