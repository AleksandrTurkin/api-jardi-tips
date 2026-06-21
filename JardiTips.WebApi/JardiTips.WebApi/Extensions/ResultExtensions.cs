using JardiTips.Domain.Common;
using JardiTips.Domain.Enums;

namespace JardiTips.WebApi.Extensions
{
    public static class ResultExtensions
    {
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
            return error.Type switch
            {
                ErrorType.NotFound => TypedResults.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: error.Code,
                    detail: error.Description),

                ErrorType.ValidationError => TypedResults.ValidationProblem(
                    errors: error.Extensions ?? new Dictionary<string, string[]>
                        { { error.Code, [error.Description] } },
                    title: "Validation Error",
                    detail: "One or more validation failures occurred."),

                ErrorType.BadRequest => TypedResults.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: error.Code,
                    detail: error.Description),

                ErrorType.EntityAlreadyExists => TypedResults.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: error.Code,
                    detail: error.Description),

                _ => TypedResults.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Server Error",
                    detail: error.Description)
            };
        }
    }
}