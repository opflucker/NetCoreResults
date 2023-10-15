namespace NetResultMonad.Tests;

public class DomainError : Error
{
    public enum Codes
    {
        Generic = 0,
        InvalidAction = 1,
    }

    public Codes Code { get; }

    private DomainError(Codes code)
    {
        Code = code;
    }

    public static readonly DomainError Generic = new(Codes.Generic);
    public static readonly DomainError InvalidAction = new(Codes.InvalidAction);
}

public class ApplicationError : Error
{
    public enum Codes
    {
        Generic = 10,
        InvalidCommand = 11,
    }

    public Codes Code { get; }

    public string? Message { get; }

    private ApplicationError(Codes code, string? message)
    {
        Code = code;
        Message = message;
    }

    public static ApplicationError Generic(string message) => new(Codes.Generic, message);
    public static readonly ApplicationError InvalidCommand = new(Codes.InvalidCommand, null);
}

public class ResultWithErrorCodeTests
{
    [Fact]
    public void When_call_failure_then_result_is_failure()
    {
        var results = new[]
        {
            Result.Failure(DomainError.InvalidAction),
            Result.Failure(ApplicationError.Generic("A generic error")),
        };

        foreach(var result in results)
        {
            Assert.True(result.IsFailure);

            switch(result.Error)
            {
                case DomainError error:
                    Assert.Equal(DomainError.Codes.InvalidAction, error.Code);
                    break;
                case ApplicationError error:
                    Assert.Equal(ApplicationError.Codes.Generic, error.Code);
                    break;
            }
        }
    }
}
