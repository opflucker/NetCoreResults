namespace NetResultMonad.Tests;

public class ResultWithDataTests
{
    [Fact]
    public void When_call_success_then_result_is_success()
    {
        var data = 1;
        var result = Result<int>.Success(data);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(data, result.Data);
    }

    [Fact]
    public void When_call_failure_then_result_is_failure()
    {
        var result = Result<int>.Failure();
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void When_convert_failure_then_result_is_failure()
    {
        var failureInt = Result<int>.Failure();
        var failure = failureInt.ToError();
        Assert.True(failure.IsFailure);

        var failureDouble = failure.ToError<double>();
        Assert.True(failureDouble.IsFailure);

        var failureDate = failureDouble.ToError<DateOnly>();
        Assert.True(failureDate.IsFailure);
    }
}
