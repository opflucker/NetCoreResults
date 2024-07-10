using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace NetCoreResults.Tests.WithWeb;

public static class ResultExtensions
{
    /// <summary>
    /// Maps a monad result to AspNetCore IResult.
    /// </summary>
    public static IActionResult ToActionResult<TData>(this Result<TData,ApplicationError> result)
        where TData : notnull
    {
        return result
            .Map<IActionResult>(
                v => new OkObjectResult(v),
                error =>
                {
                    switch (error.Code)
                    {
                        case ApplicationError.Codes.BadRequest:
                            return new BadRequestObjectResult(error.Message);
                        case ApplicationError.Codes.NotAuthorized:
                            return new ObjectResult(result.Error) { StatusCode = (int)HttpStatusCode.InternalServerError };
                        default:
                            return new UnprocessableEntityObjectResult(error.Message);
                    }
                });
    }
}

public static class SomePresentationLayerService
{
    public static Result<ApplicationError> EnsureAuthorization(string requesterName)
    {
        return requesterName == "AUTHORIZED-USER"
            ? Result.Success()
            : ApplicationError.NotAuthorized;
    }
}

public static class PresentationLayer
{
    public static IActionResult GetEntity(string requesterName, string id)
        => SomePresentationLayerService.EnsureAuthorization(requesterName)
            .MapSuccess(() => SomeApplicationService.FindById(id))
            .ToActionResult();
}
