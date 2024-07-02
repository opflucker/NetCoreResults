using System.Diagnostics.CodeAnalysis;
using static NetResults.Result;

namespace NetResults;

public static partial class Result
{
    public sealed class SuccessNoData
    {
        private SuccessNoData() {}
        public static readonly SuccessNoData Unit = new();
    }

    public static SuccessNoData Success() => SuccessNoData.Unit;
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
    public static implicit operator Result<TError>(SuccessNoData _) => successSingleton;
    public static implicit operator Result<TError>(TError error) => new(error);

    [MemberNotNullWhen(returnValue: false, nameof(error))]
    public bool IsSuccess() => success;

    [MemberNotNullWhen(returnValue: true, nameof(error))]
    public bool IsFailure() => !success;

    public bool IsFailure(out TError error)
    {
        if (IsFailure())
        {
            error = this.error;
            return true;
        }

        error = default!;
        return false;
    }

    public Result<TError> OnFailure(Action<TError> action)
    {
        if (IsFailure())
            action(error);
        return this;
    }

    public Result<TError> OnSuccess(Func<Result<TError>> func)
    {
        if (IsSuccess())
            return func();
        return this;
    }

    public Result<TError2> On<TError2>(Func<Result<TError2>> successFunc, Func<TError, TError2> failureFunc)
        where TError2 : notnull
    {
        if (IsSuccess())
            return successFunc();
        return failureFunc(error);
    }

    public T On<T>(Func<T> successFunc, Func<TError, T> failureFunc)
    {
        if (IsSuccess())
            return successFunc();
        return failureFunc(error);
    }
}

public static class ResultExtensions
{
    public static Result<TData, TError> On<TData, TError>(this Result<TError> result, Func<Result<TData, TError>> successFunc)
        where TData : notnull
        where TError : notnull
        => result.IsFailure(out var error) ? error : successFunc();
}
