namespace JardiTips.Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public ErrorDetail? Error { get; }

        protected Result(bool isSuccess, ErrorDetail? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(ErrorDetail error) => new(false, error);

        public static implicit operator Result(ErrorDetail error) => Failure(error);
    }
}
