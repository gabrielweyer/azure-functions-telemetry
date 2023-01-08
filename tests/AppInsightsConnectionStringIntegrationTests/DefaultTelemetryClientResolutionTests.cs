namespace Gabo.AzureFunctionsTelemetryAppInsightsConnectionStringIntegrationTests;

[Collection("Default")]
public sealed class DefaultTelemetryClientResolutionTests : IDisposable
{
    private readonly DefaultTelemetryFunctionClient _client;

    public DefaultTelemetryClientResolutionTests()
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

    public void Dispose()
    {
        _client.Dispose();
    }
}
