using System.Diagnostics.CodeAnalysis;
using static NetResults.Result;

namespace NetResults;

public static partial class Result
{
    /// <summary>
    /// Singleton type used to help creating a success result without data.
    /// </summary>
    public sealed class SuccessNoData
    {
        private SuccessNoData() {}
        public static readonly SuccessNoData Unit = new();
    }

    /// <summary>
    /// Helps easy access to singleton value.
    /// </summary>
    /// <returns></returns>
    public static SuccessNoData Success() => SuccessNoData.Unit;
}

/// <summary>
/// Represents a result without success data, only error data.
/// </summary>
/// <typeparam name="TError"></typeparam>
public sealed class Result<TError>
    where TError: notnull
{
    /// <summary>
    /// Signal the result as successful or failure.
    /// We can not use something like (error != default) because default value could be a valid error value.
    /// </summary>
    private readonly bool success;
    private readonly TError? error;

    public TError Error => IsFailure() ? error : throw new InvalidOperationException();

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

    /// <summary>
    /// Use of singleton object in success case because it has always the same data.
    /// </summary>
    private static readonly Result<TError> successSingleton = new();

    public static implicit operator Result<TError>(SuccessNoData _) => successSingleton;
    public static implicit operator Result<TError>(TError error) => new(error);

    [MemberNotNullWhen(returnValue: false, nameof(error))]
    public bool IsSuccess() => success;

    [MemberNotNullWhen(returnValue: true, nameof(error))]
    public bool IsFailure() => !success;

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
    /// <param name="func"></param>
    /// <returns></returns>
    public Result<TError> OnSuccess(Func<Result<TError>> func)
    {
        if (IsSuccess())
            return func();
        return this;
    }

    /// <summary>
    /// Allows perform an action on failure and return the same result reference.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result<TError> OnFailure(Action<TError> action)
    {
        if (IsFailure())
            action(error);
        return this;
    }

    /// <summary>
    /// Allows map a result to another result type, potentially changing the error type.
    /// </summary>
    /// <typeparam name="TError2"></typeparam>
    /// <param name="successFunc"></param>
    /// <param name="failureFunc"></param>
    /// <returns></returns>
    public Result<TError2> On<TError2>(Func<Result<TError2>> successFunc, Func<TError, TError2> failureFunc)
        where TError2 : notnull
    {
        if (IsSuccess())
            return successFunc();
        return failureFunc(error);
    }

    /// <summary>
    /// Allows map a result to another type, potentially a non-result type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="successFunc"></param>
    /// <param name="failureFunc"></param>
    /// <returns></returns>
    public T On<T>(Func<T> successFunc, Func<TError, T> failureFunc)
    {
        if (IsSuccess())
            return successFunc();
        return failureFunc(error);
    }
}
