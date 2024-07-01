namespace NetResults.Tests;

public class ResultWithDataTests
{
    [Fact]
    public void When_call_success_then_result_is_success()
    {
        int data = 1;
        Result<int, string> result = data;
        Assert.True(result.IsSuccess());
        Assert.False(result.IsFailure());
        if (result.IsSuccess(out int v))
            Assert.Equal(data, v);
        else
            Assert.Fail("IsSuccess returns 'false' when expected 'true'");
    }

    [Fact]
    public void When_call_failure_then_result_is_failure()
    {
        string error = "Error";
        Result<int, string> result = error;
        Assert.False(result.IsSuccess());
        Assert.True(result.IsFailure());
        if (result.IsFailure(out string e))
            Assert.Equal(error, e);
        else
            Assert.Fail("IsFailure returns 'false' when expected 'true'");
    }

    [Fact]
    public void When_convert_failure_then_result_is_failure()
    {
        Result<int, string> failureInt = "Error";
        Result<string> failure = failureInt.Narrow();
        Assert.False(failure.IsSuccess());
        Assert.True(failure.IsFailure());

        Result<double, string> failureDouble = failure.IsFailure(out string error) ? error : 0;
        Assert.True(failureDouble.IsFailure());
    }
}
