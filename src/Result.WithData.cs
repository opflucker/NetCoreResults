using System.Diagnostics.CodeAnalysis;
using static NetResults.Result;

namespace NetResults;

public static partial class Result
{
    public record struct SuccessData<TData>(TData Data);
    public record struct FailureError<TError>(TError Error);

    public static SuccessData<TData> Success<TData>(TData data) => new(data);
    public static FailureError<TError> Failure<TError>(TError error) => new(error);
}

public sealed class Result<TData, TError>
    where TData : notnull
    where TError : notnull
{
    private readonly bool success;
    private readonly TData? data;
    private readonly TError? error;

    public TData Data => IsSuccess() ? data : throw new InvalidOperationException();
    public TError Error => IsFailure() ? error : throw new InvalidOperationException();

    private Result(bool success, TData? data, TError? error)
    {
        this.success = success;
        this.data = data;
        this.error = error;
    }

    public static implicit operator Result<TData, TError>(TData data) => new(true, data, default);
    public static implicit operator Result<TData, TError>(TError error) => new(false, default, error);

    public static implicit operator Result<TData, TError>(SuccessData<TData> result) => new(true, result.Data, default);
    public static implicit operator Result<TData, TError>(FailureError<TError> result) => new(false, default, result.Error);

    [MemberNotNullWhen(returnValue: true, nameof(data))]
    [MemberNotNullWhen(returnValue: false, nameof(error))]
    public bool IsSuccess() => success;

    [MemberNotNullWhen(returnValue: true, nameof(error))]
    [MemberNotNullWhen(returnValue: false, nameof(data))]
    public bool IsFailure() => !success;

    public bool IsSuccess(out TData data)
    {
        if (IsSuccess())
        {
            data = this.data;
            return true;
        }

        data = default!;
        return false;
    }

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

    public Result<TData, TError> OnSuccess(Action<TData> action)
    {
        if (IsSuccess())
            action(data);
        return this;
    }

    public Result<TData, TError> OnFailure(Action<TError> action)
    {
        if (IsFailure())
            action(error);
        return this;
    }

    public Result<TData2, TError> OnSuccess<TData2>(Func<TData, Result<TData2, TError>> func)
        where TData2 : notnull
    {
        if (IsSuccess())
            return func(data);
        return error;
    }

    public Result<TData2, TError2> On<TData2, TError2>(Func<TData, Result<TData2, TError2>> successFunc, Func<TError, TError2> failureFunc)
        where TData2 : notnull
        where TError2 : notnull
    {
        if (IsSuccess())
            return successFunc(data);
        return failureFunc(error);
    }

    public T On<T>(Func<TData, T> successFunc, Func<TError, T> failureFunc)
    {
        if (IsSuccess())
            return successFunc(data);
        return failureFunc(error);
    }

    public Result<TError> Narrow()
    {
        if (IsFailure())
            return error;
        return Success();
    }
}
