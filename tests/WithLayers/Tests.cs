namespace NetCoreResults.Tests.WithLayers;

/// <summary>
/// Simulate an scenario where results travels implementation layers.
/// </summary>
public class Tests
{
    [Fact]
    public void When_call_is_valid_then_result_is_success()
    {
        var id = "VALID";
        var result = SomeApplicationService.SomeAction(id);
        Assert.True(result.IsSuccess());
        if (result.IsSuccess(out var value))
            Assert.Equal(id, value.Id);
    }

    [Fact]
    public void When_call_with_null_id_then_result_is_failed()
    {
        var result = SomeApplicationService.SomeAction(null);
        Assert.True(result.IsFailure());
        if (result.IsFailure(out var error))
            Assert.IsType<SomeApplicationError>(error);
    }

    [Fact]
    public void When_call_with_invalid_id_then_result_is_failed()
    {
        var result = SomeApplicationService.SomeAction("INVALID");
        Assert.True(result.IsFailure());
        if (result.IsFailure(out var error))
            Assert.IsType<SomeApplicationError>(error);
    }
}
