namespace NetCoreResults;

public abstract class Result<TError>
{
    #region Core interface

    public abstract TError Error { get; }
    public abstract bool IsSuccess();
    public abstract bool IsFailure();
    public abstract bool IsFailure(out TError error);
    public abstract Result<TError> On(Action successAction, Action<TError> failureAction);
    public abstract T Map<T>(Func<T> successFunc, Func<TError, T> failureFunc);

    public static implicit operator Result<TError>(Result.SuccessNoData _) => Success.Unit;
    public static implicit operator Result<TError>(TError error) => new Failure(error);

    #endregion

    #region Implementations

    private sealed class Success : Result<TError>
    {
        public static readonly Success Unit = new();
        private Success() { }
        public override TError Error => throw new InvalidOperationException();
        public override bool IsSuccess() => true;
        public override bool IsFailure() => false;
        public override bool IsFailure(out TError error) { error = default!; return false; }
        public override Result<TError> On(Action successAction, Action<TError> failureAction) { successAction(); return this; }
        public override T Map<T>(Func<T> successFunc, Func<TError, T> failureFunc) => successFunc();
    }

    private sealed class Failure : Result<TError>
    {
        private readonly TError error;
        public Failure(TError error) { this.error = error; }
        public override TError Error => error;
        public override bool IsSuccess() => false;
        public override bool IsFailure() => true;
        public override bool IsFailure(out TError error) { error = this.error; return true; }
        public override Result<TError> On(Action successAction, Action<TError> failureAction) { failureAction(error); return this; }
        public override T Map<T>(Func<T> successFunc, Func<TError, T> failureFunc) => failureFunc(error);
    }

    #endregion

    #region Extended interface

    public Result<TError> OnSuccess(Action action) => On(action, (_) => { });
    public Result<TError> OnFailure(Action<TError> action) => On(() => { }, action);

    public Result<TError> MapSuccess(Func<Result<TError>> func) => Map(func, error => error);
    public Result<TData, TError> MapSuccess<TData>(Func<Result<TData, TError>> func) => Map(func, error => error);
    public Result<TError2> MapFailure<TError2>(Func<TError, Result<TError2>> func) => Map(() => Result.Success(), func);
    public Result<TError2> MapFailure<TError2>(Func<TError, TError2> func) => Map<Result<TError2>>(() => Result.Success(), error => func(error));

    #endregion
}
