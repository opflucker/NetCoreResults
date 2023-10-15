namespace NetResultMonad;

public class Error
{
    public static readonly Error Unit = new();
}

public class InvalidResultUseException : Exception
{
}

public readonly struct Result
{
    public readonly Error? Error { get; }

    private Result(Error error) => Error = error;

    public bool IsSuccess => Error == null;
    public bool IsFailure => Error != null;

    public static Result Success() => new();
    public static Result Failure() => new(Error.Unit);
    public static Result Failure(Error error) => new(error);

    // helpers
    public static Result<T> Success<T>(T data) => Result<T>.Success(data);
    public static implicit operator Result(Error error) => new(error);
}

public readonly struct Result<T>
{
    public readonly T? Data { get; }
    public readonly Error? Error { get; }

    private Result(T data) => Data = data;

    private Result(Error error) => Error = error;

    public bool IsSuccess => Error == null;
    public bool IsFailure => Error != null;

    public static Result<T> Success(T data) => new(data);
    public static Result<T> Failure() => new(Error.Unit);
    public static Result<T> Failure(Error error) => new(error);

    // helpers
    public Result<V> ToError<V>() => new(Error ?? throw new InvalidResultUseException());
    public static implicit operator Result<T>(T data) => new(data);
    public static implicit operator Result<T>(Error error) => new(error);
}

public static class ResultExtensions
{
    public static Result<T> ToError<T>(this Result result) => Result<T>.Failure(result.Error ?? throw new InvalidResultUseException());
    public static Result ToError<T>(this Result<T> result) => Result.Failure(result.Error ?? throw new InvalidResultUseException());
}
