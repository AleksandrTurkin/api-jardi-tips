using JardiTips.Domain.Common;
using JardiTips.Domain.Enums;

namespace JardiTips.WebApi.Extensions
{
    public static class ResultExtensions
    {
        private const string ProblemTypeBaseUri = "/errors/";

        public static IResult ToHttpResult(this Result result)
        {
            if (result.IsSuccess)
                return TypedResults.NoContent();
            
            return MapErrorResult(result.Error!);
        }

        public static IResult ToHttpResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return TypedResults.Ok(result.Value);
           
            return MapErrorResult(result.Error!);
        }

        private static IResult MapErrorResult(ErrorDetail error)
        {
            var typeUri = ProblemTypeBaseUri + error.Code;

            if (error.Type == ErrorType.ValidationError)
            {
                return TypedResults.ValidationProblem(
                    errors: error.Extensions ?? new Dictionary<string, string[]>
                        { { error.Code, [error.Description] } },
                    title: "One or more validation errors occurred",
                    detail: error.Description,
                    type: typeUri,
                    extensions: new Dictionary<string, object?> { ["code"] = error.Code });
            }

            var (statusCode, title) = error.Type switch
            {
                ErrorType.NotFound => (StatusCodes.Status404NotFound, "Resource not found"),
                ErrorType.BadRequest => (StatusCodes.Status400BadRequest, "Bad request"),
                ErrorType.EntityAlreadyExists => (StatusCodes.Status409Conflict, "Resource already exists"),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
            };

            var extensions = new Dictionary<string, object?> { ["code"] = error.Code };

            if (error.Extensions is not null)
            {
                foreach (var (key, value) in error.Extensions)
                    extensions[key] = value;
            }

            return TypedResults.Problem(
                statusCode: statusCode,
                title: title,
                detail: error.Description,
                type: typeUri,
                extensions: extensions);
        }
    }
}