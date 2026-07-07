using System.ComponentModel.DataAnnotations;
using JardiTips.Domain.Common;
using JardiTips.Domain.Enums;

namespace JardiTips.WebApi.Extensions;

public sealed class ValidationEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (var argument in context.Arguments)
        {
            if (argument is null || IsSystemType(argument.GetType()))
                continue;

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(argument);

            if (Validator.TryValidateObject(argument, validationContext, results, validateAllProperties: true))
                continue;

            var errors = new Dictionary<string, string[]>();
            foreach (var result in results)
            {
                var message = result.ErrorMessage ?? "Invalid value.";
                var members = result.MemberNames.Any() ? result.MemberNames : [string.Empty];
                foreach (var member in members)
                    errors[member] = errors.TryGetValue(member, out var existing)
                        ? [.. existing, message]
                        : [message];
            }

            var error = new ErrorDetail(
                "validation-failed",
                "One or more validation failures occurred.",
                ErrorType.ValidationError,
                errors);

            return Result.Failure(error).ToHttpResult();
        }

        return await next(context);
    }

    private static bool IsSystemType(Type type)
    {
        return type.IsPrimitive ||
               type == typeof(string) ||
               type == typeof(CancellationToken) ||
               type == typeof(HttpContext) ||
               type == typeof(System.Security.Claims.ClaimsPrincipal) ||
               type.Namespace?.StartsWith("Microsoft.AspNetCore") == true;
    }
}