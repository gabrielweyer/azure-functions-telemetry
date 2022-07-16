namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

[Collection("Default")]
public class DefaultServiceBusTests
{
    private static readonly DefaultTelemetryFunctionClient _client;

    static DefaultServiceBusTests()
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
        var (request, _) = await _client.PollForTelemetryAsync<RequestItem>(i => i.OperationName == "ServiceBusFunction");
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
        var (request, telemetries) =
            await _client.PollForTelemetryAsync<RequestItem>(i => i.OperationName == "ServiceBusFunction");
        var executionTraces = telemetries.GetOperationItems<TraceItem>(request.OperationId);
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
            await _client.PollForTelemetryAsync<RequestItem>(i =>
                i.OperationName == "ServiceBusExceptionThrowingFunction");
        var exceptions = telemetries.GetOperationItems<ExceptionItem>(request.OperationId);
        exceptions.Should().HaveCount(3);
    }
}
