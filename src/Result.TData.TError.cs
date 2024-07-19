namespace NetCoreResults;

public abstract class Result<TData, TError>
{
    #region Core interface

    public abstract TData Data { get; }
    public abstract TError Error { get; }
    public abstract bool IsSuccess();
    public abstract bool IsFailure();
    public abstract bool IsSuccess(out TData data);
    public abstract bool IsFailure(out TError error);
    public abstract Result<TData, TError> On(Action<TData> successAction, Action<TError> failureAction);
    public abstract T Map<T>(Func<TData, T> successFunc, Func<TError, T> failureFunc);

    public static implicit operator Result<TData, TError>(TData data) => new Success(data);
    public static implicit operator Result<TData, TError>(TError error) => new Failure(error);
    public static implicit operator Result<TData, TError>(Result.SuccessData<TData> data) => new Success(data.Data);
    public static implicit operator Result<TData, TError>(Result.FailureError<TError> error) => new Failure(error.Error);
    public static bool operator true(Result<TData, TError> result) => result.IsSuccess();
    public static bool operator false(Result<TData, TError> result) => result.IsFailure();

    #endregion

    #region Implementations

    private sealed class Success : Result<TData, TError>
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

    private sealed class Failure : Result<TData, TError>
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
        public override T Map<T>(Func<TData, T> successFunc, Func<TError, T> failureFunc) => failureFunc(error);
    }

    #endregion
}
