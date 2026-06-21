using JardiTips.WebApi.Endpoints.Base;
using JardiTips.WebApi.ExceptionHandlers;
using JardiTips.WebApi.Extensions;
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
        builder.Services.AddOpenApi();

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
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
        }

        var group = app.MapGroup("api");
        app.MapEndpoints(group);

        app.UseCors("AllowAll");
        app.UseHttpsRedirection();
        
        app.UseExceptionHandler();

        app.InitializeDbIfNotExists();
    }
}

