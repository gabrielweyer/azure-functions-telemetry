namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

[Collection("Default")]
public sealed class DefaultHttpTests : IDisposable
{
    private readonly DefaultTelemetryFunctionClient _client;

    public DefaultHttpTests()
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
        var (request, telemetries) = await _client.PollForTelemetryAsync("TraceLogFunction");
        var executionTraces = telemetries.GetOperationItems<TraceItem>(request.OperationName, request.OperationId);
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
            await _client.PollForTelemetryAsync("HttpExceptionThrowingFunction");
        var exceptions = telemetries.GetOperationItems<ExceptionItem>(request.OperationName, request.OperationId);
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

    public void Dispose()
    {
        _client.Dispose();
    }
}
