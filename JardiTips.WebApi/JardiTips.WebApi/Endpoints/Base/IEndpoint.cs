namespace JardiTips.WebApi.Endpoints.Base;

public interface IEndpoint
{
    void Register(IServiceCollection services);

    void Map(IEndpointRouteBuilder builder);
}

