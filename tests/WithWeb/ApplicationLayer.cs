// Asuming a solution with a well-defined application layer, with its own errors and services

namespace NetResults.Tests.WithWeb;

public class ApplicationError
{
    public enum Codes
    {
        Generic = 10,
        BadRequest = 11,
        NotAuthorized = 12,
    }

    public Codes Code { get; }

    public string? Message { get; }

    private ApplicationError(Codes code, string? message)
    {
        Code = code;
        Message = message;
    }

    public static readonly ApplicationError Generic = new(Codes.Generic, null);
    public static ApplicationError BadRequest(string message) => new (Codes.BadRequest, message);
    public static readonly ApplicationError NotAuthorized = new (Codes.NotAuthorized, null);
}

public static class SomeApplicationService
{
    public static Result<SomeEntity, ApplicationError> FindById(string id)
    {
        if (id == "ID-FORMAT-INVALID")
            return ApplicationError.BadRequest("Id format is invalid");

        var result = SomeDomainService.GetById(id);

        return result.OnFailure<SomeEntity, DomainError, ApplicationError>(
            _ => ApplicationError.Generic);
    }
}
