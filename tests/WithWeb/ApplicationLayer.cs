namespace NetResultMonad.Tests.WithWeb;

public class ApplicationError : Error
{
    public enum Codes
    {
        Generic = 10,
        BadRequest = 11,
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
}

public static class SomeApplicationService
{
    public static Result<SomeEntity> FindById(string id)
    {
        if (id == "ID-FORMAT-INVALID")
            return ApplicationError.BadRequest("Id format is invalid");

        var result = SomeDomainService.GetById(id);
        if (result.IsFailure)
            return ApplicationError.Generic;

        return result;
    }
}
