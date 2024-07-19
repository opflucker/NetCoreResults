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
    public static bool operator true(Result<TError> result) => result.IsSuccess();
    public static bool operator false(Result<TError> result) => result.IsFailure();

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
}
