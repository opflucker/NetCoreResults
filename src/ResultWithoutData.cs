namespace NetResults;

public sealed class Result
{
    private Result() { }
    public static readonly Result Success = new();
}

public sealed class Result<TError>
{
    private readonly TError? error;

    public TError Error => error ?? throw new InvalidOperationException();

    private Result(TError? error)
    {
        this.error = error;
    }

    public static implicit operator Result<TError>(Result _) => new(default);
    public static implicit operator Result<TError>(TError error) => new(error);

    public bool IsSuccess() => error == null;
    public bool IsFailure() => error != null;

    public bool IsFailure(out TError error)
    {
        if (this.error != null)
        {
            error = this.error;
            return true;
        }

        error = default!;
        return false;
    }

    public Result<TError> OnFailure(Action<TError> action)
    {
        if (error != null)
            action(error);
        return this;
    }

    public Result<TError> OnSuccess(Func<Result<TError>> func)
    {
        if (error == null)
            return func();
        return this;
    }

    public Result<TError2> On<TError2>(Func<Result<TError2>> successFunc, Func<TError, TError2> failureFunc)
    {
        if (error == null)
            return successFunc();
        return failureFunc(error);
    }

    public T On<T>(Func<T> successFunc, Func<TError, T> failureFunc)
    {
        if (error == null)
            return successFunc();
        return failureFunc(error);
    }
}

public static class ResultExtensions
{
    public static Result<TData, TError> On<TData, TError>(this Result<TError> result, Func<Result<TData, TError>> successFunc)
        => result.IsFailure(out var error) ? error : successFunc();
}
