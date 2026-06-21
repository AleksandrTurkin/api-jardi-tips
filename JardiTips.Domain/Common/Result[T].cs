using JardiTips.Domain.Enums;

namespace JardiTips.Domain.Common
{
    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value) : base(true, null)
        {
            Value = value;
        }

        private Result(ErrorDetail error) : base(false, error)
        {
            Value = default;
        }

        public static Result<T> Success(T value) => new(value);
        public new static Result<T> Failure(ErrorDetail error) => new(error);

        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(ErrorDetail error) => Failure(error);
    }

    public record ErrorDetail(string Code, string Description, ErrorType Type, Dictionary<string, string[]>? Extensions = null);
}
