namespace NetCoreResults;

public static class Result
{
    #region No Success Data

    public sealed class SuccessNoData
    {
        private SuccessNoData() { }
        public static readonly SuccessNoData Unit = new();
    }
    public static SuccessNoData Success() => SuccessNoData.Unit;

    #endregion

    #region With Success Data

    public record struct SuccessData<TData>(TData Data);
    public record struct FailureError<TError>(TError Error);
    public static SuccessData<TData> Success<TData>(TData data) => new(data);
    public static FailureError<TError> Failure<TError>(TError error) => new(error);

    #endregion
}
