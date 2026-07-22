using JardiTips.Application.Base;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Authentication.Models;
using JardiTips.Domain.Common;
using JardiTips.WebApi.Endpoints.Base;
using JardiTips.WebApi.Extensions;

namespace JardiTips.WebApi.Endpoints;

public sealed class AuthEndpoint : IEndpoint
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<LoginCommand, Result<AuthTokenDto>>, LoginCommandHandler>();
        services.AddScoped<ICommandHandler<RefreshTokenCommand, Result<AuthTokenDto>>, RefreshTokenCommandHandler>();
        services.AddScoped<ICommandHandler<RevokeRefreshTokenCommand, Result>, RevokeRefreshTokenCommandHandler>();
    }

    public void Map(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/auth").WithTags("Auth").AllowAnonymous();

        group.MapPostCommand<LoginCommand, LoginDto, AuthTokenDto>("/login", dto => new LoginCommand(dto));
        group.MapPostCommand<RefreshTokenCommand, RefreshTokenDto, AuthTokenDto>("/refresh", dto => new RefreshTokenCommand(dto));
        group.MapPostCommandNoContent<RevokeRefreshTokenCommand, RefreshTokenDto>("/logout", dto => new RevokeRefreshTokenCommand(dto));
    }
}
