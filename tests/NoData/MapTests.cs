namespace NetCoreResults.Tests.NoData;

public class MapTests
{
    record MappingError(string Description);

    [Fact]
    public void When_Use_FuncDataToT_FuncErrorToT_On_Success_Then_Success()
    {
        Result<string> result = Result.Success();
        string mapped = result.Map(() => "Success", error => error);
        Assert.Equal("Success", mapped);
    }

    [Fact]
    public void When_Use_FuncDataToT_FuncErrorToT_On_Failure_Then_Failure()
    {
        Result<string> result = "Error";
        string mapped = result.Map(() => "Success", error => error);
        Assert.Equal("Error", mapped);
    }

    [Fact]
    public void When_Use_FuncToResult_On_Success_Then_Success()
    {
        Result<string> result = Result.Success();
        Result<string> mappedResult = result.MapSuccess(ProcessSuccess);
        Assert.True(mappedResult.IsSuccess());
    }

    [Fact]
    public void When_Use_FuncErrorToResult_On_Failure_Then_Failure()
    {
        Result<string> result = "Error";
        Result<MappingError> mappedResult = result.MapFailure(ProcessFailure);
        Assert.True(mappedResult.IsFailure());
        Assert.Equal(mappedResult.Error.Description, result.Error);
    }

    [Fact]
    public void When_Use_FuncErrorToError_On_Failure_Then_Failure()
    {
        Result<string> result = "Error";
        Result<MappingError> mappedResult = result.MapFailure(error => new MappingError(error));
        Assert.True(mappedResult.IsFailure());
        Assert.Equal(mappedResult.Error.Description, result.Error);
    }

    private Result<string> ProcessSuccess() => Result.Success();
    private Result<MappingError> ProcessFailure(string error) => new MappingError(error);
}
