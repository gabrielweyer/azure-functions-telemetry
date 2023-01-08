namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

[Collection("Custom")]
public sealed class CustomServiceBusTests : IDisposable
{
    private readonly CustomTelemetryFunctionClient _client;

    public CustomServiceBusTests()
    {
        _client = new CustomTelemetryFunctionClient();
    }

    [Fact]
    public async Task GivenServiceBusRequest_ThenUrlIsFunctionNameAnd200ResponseCode()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.TriggerServiceBusAsync();

        // Assert
        var (request, _) = await _client.PollForTelemetryAsync("ServiceBusFunction");
        request.Data.BaseData.Url.Should().Be("/ServiceBusFunction");
        request.Data.BaseData.ResponseCode.Should().Be("200");
    }

    [Fact]
    public async Task GivenServiceBusRequest_ThenDiscardExecutionAndTriggerTraces()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.TriggerServiceBusAsync();

        // Assert
        var (request, telemetries) =
            await _client.PollForTelemetryAsync("ServiceBusFunction");
        var executionTraces = telemetries.GetOperationItems<TraceItem>(request.OperationName, request.OperationId);
        executionTraces.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenServiceBusBinding_WhenException_ThenNoDuplicateExceptionTelemetry()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.TriggerServiceBusExceptionAsync();

        // Assert
        var (request, telemetries) =
            await _client.PollForTelemetryAsync("ServiceBusExceptionThrowingFunction");
        var exceptions = telemetries.GetOperationItems<ExceptionItem>(request.OperationName, request.OperationId);
        exceptions.Should().HaveCount(1);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
