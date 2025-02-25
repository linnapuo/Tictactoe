namespace Tictactoe.Engine;

public class Result<TResult, TError>
{
    public bool IsSuccess { get; }
    public bool IsError { get; }
    public TError ErrorValue { get; }
    public TResult ResultValue { get; }

    public static Result<TResult, TError> Ok(TResult value) => new(true, value, default!);
    public static Result<TResult, TError> Error(TError error) => new(false, default!, error);

    private Result(bool isSuccess, TResult value, TError error)
    {
        IsSuccess = isSuccess;
        IsError = !isSuccess;
        ResultValue = value;
        ErrorValue = error;
    }

    public Result<TResult, TError> Bind(Func<TResult, Result<TResult, TError>> func)
    {
        return IsSuccess ? func(ResultValue!) : Result<TResult, TError>.Error(ErrorValue!);
    }

    public Result<TResult, TError> Map(Func<TResult, TResult> func)
    {
        return IsSuccess ? Result<TResult, TError>.Ok(func(ResultValue!)) : Result<TResult, TError>.Error(ErrorValue!);
    }
}