using NetResults;

namespace NetResult.Tests.WithData;

public class MapSuccessTests
{
    [Fact]
    public void When_Use_FuncDataToResut_On_Success_Then_Success()
    {
        Result<int, string> result = 10;
        Result<double, string> mappedResult = result.MapSuccess(ProcessInt);
        Assert.True(mappedResult.IsSuccess());
        Assert.Equal((int)mappedResult.Data, result.Data);
    }

    [Fact]
    public void When_Use_FuncDataToData2_On_Success_Then_Success()
    {
        Result<int, string> result = 10;
        Result<double, string> mappedResult = result.MapSuccess(data => (double)data);
        Assert.True(mappedResult.IsSuccess());
        Assert.Equal((int)mappedResult.Data, result.Data);
    }

    private Result<double, string> ProcessInt(int data) => data;
}
