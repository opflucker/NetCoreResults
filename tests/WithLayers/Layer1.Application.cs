namespace NetCoreResults.Tests.WithLayers;

// And an application layer, with additional errors that are application related, not domain related
public record SomeApplicationModel(string Id);

public class SomeApplicationError
{
    public enum Codes
    {
        Generic = 10,
        InvalidCommand = 11,
    }

    public Codes Code { get; }

    public string? Message { get; }

    private SomeApplicationError(Codes code, string? message)
    {
        Code = code;
        Message = message;
    }

    public static SomeApplicationError Generic(string message) => new(Codes.Generic, message);
    public static readonly SomeApplicationError InvalidCommand = new(Codes.InvalidCommand, null);

    /// <summary>
    /// Maps a result from domain layer.
    /// </summary>
    public static SomeApplicationError From(SomeDomainError domainError) => new(Codes.Generic, $"Domain error {domainError.Code}");
}

public static class SomeApplicationService
{
    public static Result<SomeApplicationModel, SomeApplicationError> SomeAction(string? id)
    {
        if (id == null)
            return SomeApplicationError.InvalidCommand; // application layer returns its own errors

        var result = SomeDomainService.SomeAction(id);
        if (result.IsFailure(out var error))
            return SomeApplicationError.From(error); // map received errors from other layers, like domain

        return new SomeApplicationModel(result.Data.Id); // map a received model to its own model
    }
}
