namespace WingDing_Party.IdentityProvider.Domain.Common.ResultPattern;

public class Result<T>
{
    public T Value { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    private Result(T value, bool isSuccess, string error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<T> Success(T value) => 
        new(value, true, string.Empty);

    public static Result<T> Failure(string error) => 
        new(default!, false, error);

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(Error);
}
