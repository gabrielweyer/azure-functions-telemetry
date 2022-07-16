namespace Gabo.AzureFunctionsTelemetryAppInsightsConnectionStringIntegrationTests;

[Collection("Custom")]
public class CustomTelemetryClientResolutionTests
{
    private static readonly CustomTelemetryFunctionClient _client;

    static CustomTelemetryClientResolutionTests()
    {
        _client = new CustomTelemetryFunctionClient();
    }

    [Fact]
    public async Task GivenNoAppInsightsConnectionString_WhenResolvingTelemetryClient_ThenSuccess()
    {
        // Act
        var response = await _client.EmitCustomEventAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
}
