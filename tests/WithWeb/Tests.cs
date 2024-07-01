using Microsoft.AspNetCore.Mvc;

namespace NetResults.Tests.WithWeb;

public class Tests
{
    [Fact]
    public void When_Valid_Request_Then_Success()
    {
        var expectedId = "ID-VALID";
     
        var result = PresentationLayer.GetEntity("AUTHORIZED-USER", expectedId) as OkObjectResult;
        Assert.NotNull(result);
        
        var entity = result.Value as SomeEntity;
        Assert.NotNull(entity);
        Assert.Equal(expectedId, entity.Id);
    }

    [Fact]
    public void When_Not_Authorized_Then_Error()
    {
        var result = PresentationLayer.GetEntity("NOT-AUTHORIZED-USER", "ID-VALID") as ObjectResult;
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
    }

    [Fact]
    public void When_Id_Format_Invalid_Then_Error()
    {
        Assert.IsType<BadRequestObjectResult>(PresentationLayer.GetEntity("AUTHORIZED-USER", "ID-FORMAT-INVALID"));
    }

    [Fact]
    public void When_Id_Not_Found_Then_Error()
    {
        Assert.IsType<UnprocessableEntityObjectResult>(PresentationLayer.GetEntity("AUTHORIZED-USER", "ID-NOT-FOUND"));
    }
}
