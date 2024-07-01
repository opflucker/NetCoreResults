namespace NetResults;

public sealed class Result<TData, TError>
{
    private readonly object dataOrError;

    public TError Error => dataOrError is TError error ? error : throw new InvalidOperationException();
    public TData Data => dataOrError is TData data ? data : throw new InvalidOperationException();

    private Result(object dataOrError)
    {
        this.dataOrError = dataOrError;
    }

    public static implicit operator Result<TData, TError>(TData data) => new(data!);
    public static implicit operator Result<TData, TError>(TError error) => new(error!);

    public bool IsSuccess() => dataOrError is TData;
    public bool IsFailure() => dataOrError is TError;

    public bool IsSuccess(out TData data)
    {
        if (dataOrError is TData asData)
        {
            data = asData;
            return true;
        }

        data = default!;
        return false;
    }

    public bool IsFailure(out TError error)
    {
        if (dataOrError is TError asError)
        {
            error = asError;
            return true;
        }

        error = default!;
        return false;
    }

    public Result<TData, TError> OnSuccess(Action<TData> action)
    {
        if (dataOrError is TData data)
            action(data);
        return this;
    }

    public Result<TData, TError> OnFailure(Action<TError> action)
    {
        if (dataOrError is TError error)
            action(error);
        return this;
    }

    public Result<TData2, TError> OnSuccess<TData2>(Func<TData, Result<TData2, TError>> func)
    {
        if (dataOrError is TData data)
            return func(data);
        return (TError)dataOrError;
    }

    public Result<TData2, TError2> On<TData2, TError2>(Func<TData, Result<TData2, TError2>> successFunc, Func<TError, TError2> failureFunc)
    {
        if (dataOrError is TData data)
            return successFunc(data);
        return failureFunc((TError)dataOrError);
    }

    public T On<T>(Func<TData, T> successFunc, Func<TError, T> failureFunc)
    {
        if (dataOrError is TData data)
            return successFunc(data);
        return failureFunc((TError)dataOrError);
    }

    public Result<TError> Narrow()
    {
        if (dataOrError is TError error)
            return error;
        return Result.Success;
    }
}
