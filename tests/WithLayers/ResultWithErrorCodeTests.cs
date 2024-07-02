using NetResults;

namespace NetResult.Tests.WithLayers;

// Asuming a solution with a well-defined domain layer, with its own errors
public class DomainError
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

// And an application layer, with additional errors that are application related, not domain related
public class ApplicationError
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
    public static ApplicationError From(DomainError domainError) => new(Codes.Generic, $"Domain error {domainError.Code}");
}

public class ResultWithErrorCodeTests
{
    [Fact]
    public void When_call_is_valid_then_result_is_success()
    {
        var id = "VALID";
        var result = SomeApplicationLayerMethod(id);
        Assert.True(result.IsSuccess());
        if (result.IsSuccess(out var value))
            Assert.Equal(id, value.Id);
    }

    [Fact]
    public void When_call_with_null_id_then_result_is_failed()
    {
        var result = SomeApplicationLayerMethod(null);
        Assert.True(result.IsFailure());
        if (result.IsFailure(out var error))
            Assert.IsType<ApplicationError>(error);
    }

    [Fact]
    public void When_call_with_invalid_id_then_result_is_failed()
    {
        var result = SomeApplicationLayerMethod("INVALID");
        Assert.True(result.IsFailure());
        if (result.IsFailure(out var error))
            Assert.IsType<ApplicationError>(error);
    }

    // Assuming this method belongs to a service in application layer
    private static Result<SomeApplicationModel, ApplicationError> SomeApplicationLayerMethod(string? id)
    {
        if (id == null)
            return ApplicationError.InvalidCommand; // application layer returns its own errors

        return SomeDomainMethod(id)
            .On<SomeApplicationModel, ApplicationError>(
                value => new SomeApplicationModel(value.Id), // and map a received model to its own model
                ApplicationError.From); // or can pass received errors from other layers, like domain
    }

    private record SomeApplicationModel(string Id);

    // And this method belongs to a service in domain layer
    private static Result<SomeDomainModel, DomainError> SomeDomainMethod(string id)
    {
        if (id == "INVALID")
            return DomainError.InvalidAction;

        return new SomeDomainModel(id);
    }

    private record SomeDomainModel(string Id);
}
