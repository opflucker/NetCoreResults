namespace NetResults;

public abstract class Result<TError>
{
    public abstract TError Error { get; }
    public abstract bool IsSuccess();
    public abstract bool IsFailure();
    public abstract bool IsFailure(out TError error);
    public abstract Result<TError> On(Action successAction, Action<TError> failureAction);
    public abstract T Map<T>(Func<T> successFunc, Func<TError, T> failureFunc);

    public static implicit operator Result<TError>(Result.SuccessNoData _) => Success<TError>.Unit;
    public static implicit operator Result<TError>(TError error) => new Failure<TError>(error);
}

public abstract class Result<TData, TError>
{
    public abstract TData Data { get; }
    public abstract TError Error { get; }
    public abstract bool IsSuccess();
    public abstract bool IsFailure();
    public abstract bool IsSuccess(out TData data);
    public abstract bool IsFailure(out TError error);
    public abstract Result<TData, TError> On(Action<TData> successAction, Action<TError> failureAction);
    public abstract T Map<T>(Func<TData, T> successFunc, Func<TError, T> failureFunc);

    public static implicit operator Result<TData, TError>(TData data) => new Success<TData, TError>(data);
    public static implicit operator Result<TData, TError>(TError error) => new Failure<TData, TError>(error);
    public static implicit operator Result<TData, TError>(Result.SuccessData<TData> data) => new Success<TData, TError>(data.Data);
    public static implicit operator Result<TData, TError>(Result.FailureError<TError> error) => new Failure<TData, TError>(error.Error);
}
