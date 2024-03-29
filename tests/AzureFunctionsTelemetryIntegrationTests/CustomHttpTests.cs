namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

[Collection("Custom")]
public sealed class CustomHttpTests : IDisposable
{
    private readonly CustomTelemetryFunctionClient _client;

    public CustomHttpTests()
    {
        _client = new CustomTelemetryFunctionClient();
    }

    [Fact]
    public async Task GivenHttpRequest_ThenDiscardExecutionTraces()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.LogVerboseAsync();

        // Assert
        var (request, telemetries) =
            await _client.PollForTelemetryAsync("TraceLogFunction");
        var executionTraces = telemetries.GetOperationItems<TraceItem>(request.OperationName, request.OperationId);
        executionTraces.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenHttpBinding_WhenException_ThenNoDuplicateExceptionTelemetry()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.ThrowOnHttpBindingAsync();

        // Assert
        var (request, telemetries) =
            await _client.PollForTelemetryAsync("HttpExceptionThrowingFunction");
        var exceptions = telemetries.GetOperationItems<ExceptionItem>(request.OperationName, request.OperationId);
        exceptions.Should().HaveCount(1);
    }

    [Fact]
    public async Task GivenHealthCheck_ThenRequestIsDiscarded()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.CheckHealthAsync();
        await _client.LogVerboseAsync();

        // Assert
        var telemetries = await _client.GetTelemetryAsync();
        var requests = telemetries.Where(i => i is RequestItem).Cast<RequestItem>().ToList();
        requests.Should().NotContain(r => r.OperationName == "HealthFunction");
        requests.Should().Contain(r => r.OperationName == "TraceLogFunction");
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
