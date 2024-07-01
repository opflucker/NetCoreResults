namespace NetResults.Tests;

public class ResultTests
{
    [Fact]
    public void When_call_success_then_result_is_success()
    {
        Result<string> result = Result.Success;
        Assert.True(result.IsSuccess());
        Assert.False(result.IsFailure());
    }

    [Fact]
    public void When_call_failure_then_result_is_failure()
    {
        Result<string> result = "Failure reason";
        Assert.False(result.IsSuccess());
        Assert.True(result.IsFailure());
    }

}