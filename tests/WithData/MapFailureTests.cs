namespace NetCoreResults.Tests.WithData;

public class MapFailureTests
{
    record MappingError(string Description);
    
    [Fact]
    public void When_Use_FuncErrorToResut_On_Failure_Then_Failure()
    {
        Result<int, string> result = "Error";
        Result<int, MappingError> mappedResult = result.MapFailure(ProcessStringError);
        Assert.True(mappedResult.IsFailure());
        Assert.Equal(mappedResult.Error.Description, result.Error);
    }

    [Fact]
    public void When_Use_FuncErrorToError2_On_Failure_Then_Failure()
    {
        Result<int, string> result = "Error";
        Result<int, MappingError> mappedResult = result.MapFailure(error => new MappingError(error));
        Assert.True(mappedResult.IsFailure());
        Assert.Equal(mappedResult.Error.Description, result.Error);
    }

    private Result<int, MappingError> ProcessStringError(string error) => new MappingError(error);
}