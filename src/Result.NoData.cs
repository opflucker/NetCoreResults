using static NetResults.Result;

namespace NetResults;

public static partial class Result
{
    public sealed class Success
    {
        private Success() {}
        public static readonly Success Unit = new();
    }
}

public sealed class Result<TError>
    where TError: notnull
{
    private readonly bool success;
    private readonly TError? error;

    public TError Error => !success ? error! : throw new InvalidOperationException();

    private Result()
    {
        success = true;
        error = default;
    }

    private Result(TError error)
    {
        success = false;
        this.error = error;
    }

    private static readonly Result<TError> successSingleton = new();
    public static implicit operator Result<TError>(Success _) => successSingleton;
    public static implicit operator Result<TError>(TError error) => new(error);

    public bool IsSuccess() => success;
    public bool IsFailure() => !success;

    public bool IsFailure(out TError error)
    {
        if (!success)
        {
            error = this.error!;
            return true;
        }

        error = default!;
        return false;
    }

    public Result<TError> OnFailure(Action<TError> action)
    {
        if (!success)
            action(error!);
        return this;
    }

    public Result<TError> OnSuccess(Func<Result<TError>> func)
    {
        if (success)
            return func();
        return this;
    }

    public Result<TError2> On<TError2>(Func<Result<TError2>> successFunc, Func<TError, TError2> failureFunc)
        where TError2 : notnull
    {
        if (success)
            return successFunc();
        return failureFunc(error!);
    }

    public T On<T>(Func<T> successFunc, Func<TError, T> failureFunc)
    {
        if (success)
            return successFunc();
        return failureFunc(error!);
    }
}

public static class ResultExtensions
{
    public static Result<TData, TError> On<TData, TError>(this Result<TError> result, Func<Result<TData, TError>> successFunc)
        where TData : notnull
        where TError : notnull
        => result.IsFailure(out var error) ? error! : successFunc();
}
