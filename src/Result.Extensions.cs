namespace NetResults;

public static class ResultExtensions
{
    /// <summary>
    /// Allows map a Result&lt;TError&gt; to Result&lt;TData,TError&gt;, it is, a result with data keeping the same error type and value.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <param name="result"></param>
    /// <param name="successFunc"></param>
    /// <returns></returns>
    public static Result<TData, TError> On<TData, TError>(this Result<TError> result, Func<Result<TData, TError>> successFunc)
        where TData : notnull
        where TError : notnull
        => result.IsFailure(out var error) ? error : successFunc();
}
