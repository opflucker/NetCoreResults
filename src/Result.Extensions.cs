namespace NetCoreResults;

public static partial class ResultExtensions
{
    #region No Success Data

    public static Result<TError> OnSuccess<TError>(this Result<TError>  result, Action action) => result.On(action, (_) => { });
    public static Result<TError> OnFailure<TError>(this Result<TError> result, Action<TError> action) => result.On(() => { }, action);

    public static Result<TError> MapSuccess<TError>(this Result<TError> result, Func<Result<TError>> func) => result.Map(func, error => error);
    public static Result<TData, TError> MapSuccess<TData, TError>(this Result<TError> result, Func<Result<TData, TError>> func) => result.Map(func, error => error);
    public static Result<TError2> MapFailure<TError, TError2>(this Result<TError> result, Func<TError, Result<TError2>> func) => result.Map(() => Result.Success(), func);
    public static Result<TError2> MapFailure<TError, TError2>(this Result<TError> result, Func<TError, TError2> func) => result.Map<Result<TError2>>(() => Result.Success(), error => func(error));

    #endregion

    #region With Success Data

    public static Result<TData, TError> OnSuccess<TData, TError>(this Result<TData, TError> result, Action<TData> action) => result.On(action, _ => { });
    public static Result<TData, TError> OnFailure<TData, TError>(this Result<TData, TError> result, Action<TError> action) => result.On(_ => { }, action);

    public static Result<TData2, TError> MapSuccess<TData, TError, TData2>(this Result<TData, TError> result, Func<TData, Result<TData2, TError>> func) => result.Map(func, error => error);
    public static Result<TData2, TError> MapSuccess<TData, TError, TData2>(this Result<TData, TError> result, Func<TData, TData2> func) => result.Map<Result<TData2, TError>>(data => func(data), error => error);

    public static Result<TData, TError2> MapFailure<TData, TError, TError2>(this Result<TData, TError> result, Func<TError, Result<TData, TError2>> func) => result.Map(data => data, func);
    public static Result<TData, TError2> MapFailure<TData, TError, TError2>(this Result<TData, TError> result, Func<TError, TError2> func) => result.Map<Result<TData, TError2>>(data => data, error => func(error));

    public static Result<TError> TrimSuccess<TData, TError>(this Result<TData, TError> result) => result.Map<Result<TError>>(_ => Result.Success(), error => error);

    #endregion
}