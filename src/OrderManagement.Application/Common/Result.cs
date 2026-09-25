namespace OrderManagement.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }

        public string? Error { get; }

        public T? Value { get; }
        public ResultErrorType? ErrorType { get; }

        private Result(bool isSuccess, T? value, string? error, ResultErrorType? errorType)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            ErrorType = errorType;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, null, null);
        }

        public static Result<T> Failure(string error, ResultErrorType errorType)
        {
            return new Result<T>(false, default, error, errorType);
        }
    }
}
