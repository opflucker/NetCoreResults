using NetResults;

namespace NetResult.Tests.NoData;

public class StringErrorTests
{
    [Fact]
    public void When_initialize_with_success_then_result_is_success()
    {
        Result<string> result = Result.Success();
        Assert.True(result.IsSuccess());
        Assert.False(result.IsFailure());
        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void When_initialize_with_error_then_result_is_failure()
    {
        const string error = "Failure reason";
        Result<string> result = error;
        Assert.False(result.IsSuccess());
        Assert.True(result.IsFailure());
        Assert.Equal(error, result.Error);
    }

}
