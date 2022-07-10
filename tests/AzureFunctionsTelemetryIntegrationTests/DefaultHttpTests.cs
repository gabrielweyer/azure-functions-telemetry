namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

[Collection("Default")]
public class DefaultHttpTests
{
    private static readonly DefaultTelemetryFunctionClient _client;

    static DefaultHttpTests()
    {
        _client = new DefaultTelemetryFunctionClient();
    }

    [Fact]
    public async Task GivenHttpRequest_ThenTwoExecutionTracesAreEmitted()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.LogVerboseAsync();

        // Assert
        var (request, telemetries) =
            await _client.PollForTelemetryAsync<RequestItem>(i => i.OperationName == "TraceLogFunction");
        var executionTraces = telemetries.GetOperationItems<TraceItem>(request.OperationId);
        executionTraces.Should().HaveCount(2);
    }

    [Fact]
    public async Task GivenHttpBinding_WhenException_ThenDuplicateExceptionTelemetry()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.ThrowOnHttpBindingAsync();

        // Assert
        var (request, telemetries) =
            await _client.PollForTelemetryAsync<RequestItem>(i =>
                i.OperationName == "HttpExceptionThrowingFunction");
        var exceptions = telemetries.GetOperationItems<ExceptionItem>(request.OperationId);
        exceptions.Should().HaveCount(2);
    }

    [Fact]
    public async Task GivenHealthCheck_ThenRequestIsNotDiscarded()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.CheckHealthAsync();
        await _client.LogVerboseAsync();

        // Assert
        var telemetries = await _client.GetTelemetryAsync();
        var requests = telemetries.Where(i => i is RequestItem).Cast<RequestItem>().ToList();
        requests.Should().Contain(i => i.OperationName == "HealthFunction");
        requests.Should().Contain(i => i.OperationName == "TraceLogFunction");
    }
}
