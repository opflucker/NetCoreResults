// Asuming a solution with a well-defined domain layer, with its own errors and services

namespace NetCoreResults.Tests.WithWeb;

public class DomainError
{
    public enum Codes
    {
        EntityNotFound = 0,
    }

    public Codes Code { get; }

    private DomainError(Codes code)
    {
        Code = code;
    }

    public static readonly DomainError EntityNotFound = new(Codes.EntityNotFound);
}

public record class SomeEntity(string Id);

public static class SomeDomainService
{
    public static Result<SomeEntity, DomainError> GetById(string id)
    {
        if (id == "ID-NOT-FOUND")
            return DomainError.EntityNotFound;

        return new SomeEntity(id);
    }
}