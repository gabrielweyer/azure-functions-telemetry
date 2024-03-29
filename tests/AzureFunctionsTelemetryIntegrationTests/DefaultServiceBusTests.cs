namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

[Collection("Default")]
public sealed class DefaultServiceBusTests : IDisposable
{
    private readonly DefaultTelemetryFunctionClient _client;

    public DefaultServiceBusTests()
    {
        _client = new DefaultTelemetryFunctionClient();
    }

    [Fact]
    public async Task GivenServiceBusRequest_ThenNullUrlAndZeroResponseCode()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.TriggerServiceBusAsync();

        // Assert
        var (request, _) = await _client.PollForTelemetryAsync("ServiceBusFunction");
        Assert.Null(request.Data.BaseData.Url);
        request.Data.BaseData.ResponseCode.Should().Be("0");
    }

    [Fact]
    public async Task GivenServiceBusRequest_ThenExecutionAndTriggerTracesAreEmitted()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.TriggerServiceBusAsync();

        // Assert
        var (request, telemetries) = await _client.PollForTelemetryAsync("ServiceBusFunction");
        var executionTraces = telemetries.GetOperationItems<TraceItem>(request.OperationName, request.OperationId);
        executionTraces.Should().HaveCount(3);
    }

    [Fact]
    public async Task GivenServiceBusBinding_WhenException_ThenTriplicateExceptionTelemetry()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.TriggerServiceBusExceptionAsync();

        // Assert
        var (request, telemetries) =
            await _client.PollForTelemetryAsync("ServiceBusExceptionThrowingFunction");
        var exceptions = telemetries.GetOperationItems<ExceptionItem>(request.OperationName, request.OperationId);
        exceptions.Should().HaveCount(3);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
