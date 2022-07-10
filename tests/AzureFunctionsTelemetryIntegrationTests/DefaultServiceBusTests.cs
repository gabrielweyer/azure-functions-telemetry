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
    public async Task GivenServiceBusRequest_ThenNullUrlAndZeroStatusCode()
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
}
