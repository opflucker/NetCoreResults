using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace NetResultMonad.Tests.WithWeb;

public static class ResultExtensions
{
    public static Result<V> OnSuccess<V>(this Result result, Func<Result<V>> action)
    {
        if (result.IsFailure)
            return result.ToError<V>();

        return action();
    }

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

public static class OtherService
{
    public static Result GetAuthorization(string requesterName)
    {
        return requesterName == "AUTHORIZED-USER"
            ? Result.Success()
            : Result.Failure();
    }
}

public static class WebLayer
{
    public static IActionResult GetEntity(string requesterName, string id)
    {
        return OtherService.GetAuthorization(requesterName)
            .OnSuccess(() => SomeApplicationService.FindById(id))
            .ToActionResult();
    }
}
