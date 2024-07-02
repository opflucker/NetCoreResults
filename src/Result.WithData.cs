using System.Diagnostics.CodeAnalysis;
using static NetResults.Result;

namespace NetResults;

public static partial class Result
{
    /// <summary>
    /// Helps create success results when success and error types used are the same.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="Data"></param>
    public record struct SuccessData<TData>(TData Data);

    /// <summary>
    /// Helps create failure results when success and error types used are the same.
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <param name="Error"></param>
    public record struct FailureError<TError>(TError Error);

    /// <summary>
    /// Helps easy creating a SuccessData instance.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static SuccessData<TData> Success<TData>(TData data) => new(data);

    /// <summary>
    /// Helps easy creating a FailureError instance.
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <param name="error"></param>
    /// <returns></returns>
    public static FailureError<TError> Failure<TError>(TError error) => new(error);
}

/// <summary>
/// Represents a result with success and error data.
/// </summary>
/// <typeparam name="TData"></typeparam>
/// <typeparam name="TError"></typeparam>
public sealed class Result<TData, TError>
    where TData : notnull
    where TError : notnull
{
    /// <summary>
    /// Signal the result as successful or failure.
    /// We can not use something like (error != default) or (data != default) because default values could be valids.
    /// </summary>
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

    /// <summary>
    /// Enables a secure way to access success data. Similar approach to 'Try' methods, like int.TryParse.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Enables a secure way to access error data. Similar approach to 'Try' methods, like int.TryParse.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Allows perform an action on success and return the same result reference.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result<TData, TError> OnSuccess(Action<TData> action)
    {
        if (IsSuccess())
            action(data);
        return this;
    }

    /// <summary>
    /// Allows perform an action on failure and return the same result reference.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result<TData, TError> OnFailure(Action<TError> action)
    {
        if (IsFailure())
            action(error);
        return this;
    }

    /// <summary>
    /// Allows map a Result&lt;TData1,TError&gt; to Result&lt;TData2,TError&gt;, it is, keeping the same error type and value.
    /// </summary>
    /// <typeparam name="TData2"></typeparam>
    /// <param name="func"></param>
    /// <returns></returns>
    public Result<TData2, TError> OnSuccess<TData2>(Func<TData, Result<TData2, TError>> func)
        where TData2 : notnull
    {
        if (IsSuccess())
            return func(data);
        return error;
    }

    /// <summary>
    /// Allows map a Result&lt;TData1,TError1&gt; to Result&lt;TData2,TError2&gt;, portentially changing success and error types and values.
    /// </summary>
    /// <typeparam name="TData2"></typeparam>
    /// <typeparam name="TError2"></typeparam>
    /// <param name="successFunc"></param>
    /// <param name="failureFunc"></param>
    /// <returns></returns>
    public Result<TData2, TError2> On<TData2, TError2>(Func<TData, Result<TData2, TError2>> successFunc, Func<TError, TError2> failureFunc)
        where TData2 : notnull
        where TError2 : notnull
    {
        if (IsSuccess())
            return successFunc(data);
        return failureFunc(error);
    }

    /// <summary>
    /// Allows map a result to another type, potentially a non-result type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="successFunc"></param>
    /// <param name="failureFunc"></param>
    /// <returns></returns>
    public T On<T>(Func<TData, T> successFunc, Func<TError, T> failureFunc)
    {
        if (IsSuccess())
            return successFunc(data);
        return failureFunc(error);
    }

    /// <summary>
    /// Allows map a Result&lt;TData,TError&gt; to Result&lt;TError&gt;, it is, discarting success data.
    /// </summary>
    /// <returns></returns>
    public Result<TError> Narrow()
    {
        if (IsFailure())
            return error;
        return Success();
    }
}
