using NetResults;

namespace NetResult.Tests.NoData;

public class IntErrorTests
{
    [Fact]
    public void When_initialize_with_success_then_result_is_success()
    {
        Result<int> result = Result.Success();
        Assert.True(result.IsSuccess());
        Assert.False(result.IsFailure());
        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void When_initialize_with_error_then_result_is_failure()
    {
        const int error = 10;
        Result<int> result = error;
        Assert.False(result.IsSuccess());
        Assert.True(result.IsFailure());
        Assert.Equal(error, result.Error);
    }

}