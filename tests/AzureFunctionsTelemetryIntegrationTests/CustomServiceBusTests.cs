namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

[Collection("Custom")]
public class CustomServiceBusTests
{
    private static readonly CustomTelemetryFunctionClient _client;

    static CustomServiceBusTests()
    {
        _client = new CustomTelemetryFunctionClient();
    }

    [Fact]
    public async Task GivenServiceBusRequest_ThenUrlIsFunctionNameAnd200StatusCode()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.TriggerServiceBusAsync();

        // Assert
        var (request, _) = await _client.PollForTelemetryAsync<RequestItem>(i => i.OperationName == "ServiceBusFunction");
        request.Data.BaseData.Url.Should().Be("/ServiceBusFunction");
        request.Data.BaseData.ResponseCode.Should().Be("200");
    }

    [Fact]
    public async Task GivenServiceBusRequest_ThenDiscardExecutionTraces()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.TriggerServiceBusAsync();

        // Assert
        var (request, telemetries) =
            await _client.PollForTelemetryAsync<RequestItem>(i => i.OperationName == "ServiceBusFunction");
        var executionTraces = telemetries.GetOperationItems<TraceItem>(request.OperationId);
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
            await _client.PollForTelemetryAsync<RequestItem>(i =>
                i.OperationName == "ServiceBusExceptionThrowingFunction");
        var exceptions = telemetries.GetOperationItems<ExceptionItem>(request.OperationId);
        exceptions.Should().HaveCount(1);
    }
}
