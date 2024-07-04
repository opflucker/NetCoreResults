namespace NetResults;

public static partial class ResultExtensions
{
    #region No Data

    public static Result<TError2> On<TError, TError2>(
        this Result<TError> result,
        Func<Result<TError2>> successFunc,
        Func<TError, Result<TError2>> failureFunc)
        => result.On(successFunc, failureFunc);
    public static Result<TError> OnSuccess<TError>(this Result<TError> result, Action action)
        => result.On(action, (_) => { });
    public static Result<TError> OnFailure<TError>(this Result<TError> result, Action<TError> action)
        => result.On(() => { }, action);

    #endregion

    #region With Data

    public static Result<TData, TError> OnSuccess<TData, TError>(this Result<TData, TError> result, Action<TData> action)
        => result.On(action, (_) => { });
    public static Result<TData, TError> OnFailure<TData, TError>(this Result<TData, TError> result, Action<TError> action)
        => result.On((_) => { }, action);
    public static Result<TData2, TError2> On<TData, TError, TData2, TError2>(
        this Result<TData, TError> result,
        Func<TData, Result<TData2, TError2>> successFunc,
        Func<TError, Result<TData2, TError2>> failureFunc)
        => result.On(successFunc, failureFunc);
    public static Result<TData2, TError> OnSuccess<TData, TError, TData2>(this Result<TData, TError> result, Func<TData, Result<TData2, TError>> func)
        => result.On(func, (_) => result.Error);
    public static Result<TData, TError2> OnFailure<TData, TError, TError2>(this Result<TData, TError> result, Func<TError, Result<TData, TError2>> func)
        => result.On(data => data, func);
    public static Result<TError> Narrow<TData, TError>(this Result<TData, TError> result)
        => result.On<Result<TError>>((_) => Result.Success(), (error) => error);

    #endregion
}
