namespace NetResults;

public sealed class Failure<TError> : Result<TError>
{
    private readonly TError error;
    public Failure(TError error) { this.error = error; }
    public override TError Error => error;
    public override bool IsSuccess() => false;
    public override bool IsFailure() => true;
    public override bool IsFailure(out TError error) { error = this.error; return true; }
    public override Result<TError> On(Action successAction, Action<TError> failureAction) { failureAction(error); return this; }
    public override T On<T>(Func<T> successFunc, Func<TError, T> failureFunc) => failureFunc(error);
}

public sealed class Failure<TData, TError> : Result<TData, TError>
{
    private readonly TError error;
    public Failure(TError error) { this.error = error; }
    public override TData Data => throw new InvalidOperationException();
    public override TError Error => error;
    public override bool IsSuccess() => false;
    public override bool IsFailure() => true;
    public override bool IsSuccess(out TData data) { data = default!; return false; }
    public override bool IsFailure(out TError error) { error = this.error; return true; }
    public override Result<TData, TError> On(Action<TData> successAction, Action<TError> failureAction) { failureAction(error); return this; }
    public override T On<T>(Func<TData, T> successFunc, Func<TError, T> failureFunc) => failureFunc(error);
}
