using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace JardiTips.WebApi.ExceptionHandlers;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred");

        var (statusCode, type, title, detail) = exception switch
        {
            ApplicationException => (
                StatusCodes.Status400BadRequest,
                "/errors/bad-request",
                "Bad request",
                exception.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "/errors/internal-server-error",
                "An unexpected error occurred",
                "An unexpected error occurred while processing your request.")
        };

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = type,
                Title = title,
                Detail = detail,
                Status = statusCode,
                Instance = httpContext.Request.Path
            }
        });
    }
}

