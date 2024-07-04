namespace NetResults;

public sealed class Success<TError> : Result<TError>
{
    public static readonly Success<TError> Unit = new();
    private Success() { }
    public override TError Error => throw new InvalidOperationException();
    public override bool IsSuccess() => true;
    public override bool IsFailure() => false;
    public override bool IsFailure(out TError error) { error = default!; return false; }
    public override Result<TError> On(Action successAction, Action<TError> failureAction) { successAction(); return this; }
    public override T Map<T>(Func<T> successFunc, Func<TError, T> failureFunc) => successFunc();
}

public sealed class Success<TData, TError> : Result<TData, TError>
{
    private readonly TData data;
    public Success(TData data) { this.data = data; }
    public override TData Data => data;
    public override TError Error => throw new InvalidOperationException();
    public override bool IsSuccess() => true;
    public override bool IsFailure() => false;
    public override bool IsSuccess(out TData data) { data = this.data; return true; }
    public override bool IsFailure(out TError error) { error = default!; return false; }
    public override Result<TData, TError> On(Action<TData> successAction, Action<TError> failureAction) { successAction(data); return this; }
    public override T Map<T>(Func<TData, T> successFunc, Func<TError, T> failureFunc) => successFunc(data);
}
