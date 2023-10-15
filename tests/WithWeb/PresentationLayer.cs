// Asuming a solution with a well-defined presentation layer

using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace NetResultMonad.Tests.WithWeb;

public static class ResultExtensions
{
    // This is a common helper that allows fluent-style use of results
    public static Result<V> OnSuccess<V>(this Result result, Func<Result<V>> action)
    {
        if (result.IsFailure)
            return result.ToError<V>();

        return action();
    }

    // This is a common helper for web-applications that handle result monads
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Data);

        switch (result.Error)
        {
            case ApplicationError error:
                switch (error.Code)
                {
                    case ApplicationError.Codes.BadRequest:
                        return new BadRequestObjectResult(error.Message);
                    default:
                        return new UnprocessableEntityObjectResult(error.Message);
                }
            default:
                return new ObjectResult(result.Error) { StatusCode = (int)HttpStatusCode.InternalServerError };
        }
    }
}

public static class SomePresentationLayerService
{
    public static Result EnsureAuthorization(string requesterName)
    {
        return requesterName == "AUTHORIZED-USER"
            ? Result.Success()
            : Result.Failure();
    }
}

public static class PresentationLayer
{
    // This method represents an action-method of some web controller
    public static IActionResult GetEntity(string requesterName, string id)
    {
        return SomePresentationLayerService.EnsureAuthorization(requesterName)
            .OnSuccess(() => SomeApplicationService.FindById(id))
            .ToActionResult();
    }
}
