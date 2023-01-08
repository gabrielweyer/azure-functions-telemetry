namespace Gabo.AzureFunctionsTelemetryAppInsightsConnectionStringIntegrationTests;

[Collection("Custom")]
public sealed class CustomTelemetryClientResolutionTests : IDisposable
{
    private readonly CustomTelemetryFunctionClient _client;

    public CustomTelemetryClientResolutionTests()
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

    public void Dispose()
    {
        _client.Dispose();
    }
}
