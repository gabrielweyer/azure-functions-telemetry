namespace Gabo.AzureFunctionsTelemetryAppInsightsConnectionStringIntegrationTests;

[Collection("Default")]
public class DefaultTelemetryClientResolutionTests
{
    private static readonly DefaultTelemetryFunctionClient _client;

    static DefaultTelemetryClientResolutionTests()
    {
        _client = new DefaultTelemetryFunctionClient();
    }

    [Fact]
    public async Task GivenNoAppInsightsConnectionString_WhenResolvingTelemetryClient_ThenError()
    {
        // Act
        var response = await _client.EmitCustomEventAsync();

        // Assert
        Assert.False(response.IsSuccessStatusCode);
    }
}
