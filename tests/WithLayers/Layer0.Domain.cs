namespace NetCoreResults.Tests.WithLayers;

public record SomeDomainModel(string Id);

public class SomeDomainError
{
    public enum Codes
    {
        Generic = 0,
        InvalidAction = 1,
    }

    public Codes Code { get; }

    private SomeDomainError(Codes code)
    {
        Code = code;
    }

    public static readonly SomeDomainError Generic = new(Codes.Generic);
    public static readonly SomeDomainError InvalidAction = new(Codes.InvalidAction);
}

public static class SomeDomainService
{
    public static Result<SomeDomainModel, SomeDomainError> SomeAction(string id)
    {
        if (id == "INVALID")
            return SomeDomainError.InvalidAction;

        return new SomeDomainModel(id);
    }
}