using System.Reflection;

namespace JardiTips.WebApi.Endpoints.Base;

public static class EndpointConfigurationExtension
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var endpoints = assembly.DefinedTypes
            .Where(x => x.IsAbstract == false &&
                        x.IsInterface == false &&
                        x.IsAssignableTo(typeof(IEndpoint)))
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>();
        
        foreach (var item in endpoints)
        {
            services.AddSingleton(item);
            item.Register(services);
        }

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder == null
            ? app
            : routeGroupBuilder;

        foreach (var item in endpoints)
            item.Map(builder);

        return app;
    }
}

