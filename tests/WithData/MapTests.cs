namespace NetCoreResults.Tests.WithData;

public class MapTests
{
    record MappingError(string Description);

    [Fact]
    public void When_Use_FuncDataToResult_FuncErrorToResult_On_Success_Then_Success()
    {
        Result<int, string> result = 10;
        Result<string, MappingError> mappedResult = result.Map(ProcessInt, ProcessStringError);
        Assert.True(mappedResult.IsSuccess());
        Assert.Equal(mappedResult.Data, result.Data.ToString());
    }

    [Fact]
    public void When_Use_FuncDataToData2_FuncErrorToResult_On_Success_Then_Success()
    {
        Result<int, string> result = 10;
        Result<string, MappingError> mappedResult = result.Map(data => data.ToString(), ProcessStringError);
        Assert.True(mappedResult.IsSuccess());
        Assert.Equal(mappedResult.Data, result.Data.ToString());
    }

    [Fact]
    public void When_Use_FuncDataToResult_FuncErrorToError2_On_Success_Then_Success()
    {
        Result<int, string> result = 10;
        Result<string, MappingError> mappedResult = result.Map(ProcessInt, error => new MappingError(error));
        Assert.True(mappedResult.IsSuccess());
        Assert.Equal(mappedResult.Data, result.Data.ToString());
    }

    [Fact]
    public void When_Use_FuncDataToData2_FuncErrorToError2_On_Success_Then_Success()
    {
        Result<int, string> result = 10;
        Result<string, MappingError> mappedResult = result.Map<Result<string, MappingError>>(data => data.ToString(), error => new MappingError(error));
        Assert.True(mappedResult.IsSuccess());
        Assert.Equal(mappedResult.Data, result.Data.ToString());
    }

    private Result<string, MappingError> ProcessInt(int data) => data.ToString();
    private Result<string, MappingError> ProcessStringError(string error) => new MappingError(error);
}
