namespace NetResultMonad.Tests;

public class ResultTests
{
    [Fact]
    public void When_call_success_then_result_is_success()
    {
        var result = Result.Success();
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void When_call_failure_then_result_is_failure()
    {
        var result = Result.Failure();
        Assert.True(result.IsFailure);
    }

}