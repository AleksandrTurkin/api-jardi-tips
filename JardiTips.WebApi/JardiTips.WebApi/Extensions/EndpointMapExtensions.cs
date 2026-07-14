using JardiTips.Application.Base;
using JardiTips.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JardiTips.WebApi.Extensions;

public static class EndpointMapExtensions
{
    public static RouteHandlerBuilder MapPostCommand<TRequest, TDto>(this IEndpointRouteBuilder builder, string pattern, Func<TDto, TRequest> create)
        where TRequest : class
        => builder.MapPost(pattern,
            [AllowAnonymous] async (
                [FromBody] TDto dto,
                [FromServices] ICommandHandler<TRequest, Result<Guid>> handler,
                CancellationToken cancellationToken) =>
            {
                var request = create(dto);
                var result = await handler.HandleAsync(request, cancellationToken);
                return result.ToHttpResult();
            }).AddEndpointFilter<ValidationEndpointFilter>();

    public static RouteHandlerBuilder MapGetByIdQuery<TRequest, TResponse, TKey>(this IEndpointRouteBuilder builder, string pattern, Func<TKey, TRequest> create)
        where TRequest : class
        where TResponse : class
        => builder.MapGet(pattern,
            [AllowAnonymous] async (
                TKey id,
                [FromServices] IQueryHandler<TRequest, Result<TResponse>> handler,
                CancellationToken cancellationToken) =>
            {
                var request = create(id);
                var result = await handler.HandleAsync(request, cancellationToken);
                return result.ToHttpResult();
            });

    public static RouteHandlerBuilder MapGetFilterQuery<TRequest, TResponse, TFilter>(this IEndpointRouteBuilder builder, string pattern, Func<TFilter, TRequest> create)
        where TRequest : class
        where TResponse : class
        where TFilter : PagedRequestDto
        => builder.MapGet(pattern,
            [AllowAnonymous] async (
                [AsParameters] TFilter filter,
                [FromServices] IQueryHandler<TRequest, Result<TResponse>> handler,
                CancellationToken cancellationToken) =>
            {
                var request = create(filter);
                var result = await handler.HandleAsync(request, cancellationToken);
                return result.ToHttpResult();
            }).AddEndpointFilter<ValidationEndpointFilter>();

    public static RouteHandlerBuilder MapPutCommand<TRequest, TDto, TKey>(this IEndpointRouteBuilder builder, string pattern, Func<TKey, TDto, TRequest> create)
        where TRequest : class
        => builder.MapPut(pattern,
            [AllowAnonymous] async (
                TKey id,
                [FromBody] TDto dto,
                [FromServices] ICommandHandler<TRequest, Result> handler,
                CancellationToken cancellationToken) =>
            {
                var request = create(id, dto);
                var result = await handler.HandleAsync(request, cancellationToken);
                return result.ToHttpResult();
            }).AddEndpointFilter<ValidationEndpointFilter>();


    public static RouteHandlerBuilder MapDeleteCommand<TRequest, TKey>(this IEndpointRouteBuilder builder, string pattern,
        Func<TKey, TRequest> create)
        where TRequest : class
        => builder.MapDelete(pattern, async (
                TKey id,
                [FromServices] ICommandHandler<TRequest, Result> handler,
                CancellationToken cancellationToken) =>
        {
            var request = create(id);
            var result = await handler.HandleAsync(request, cancellationToken);
            return result.ToHttpResult();
        });
}

