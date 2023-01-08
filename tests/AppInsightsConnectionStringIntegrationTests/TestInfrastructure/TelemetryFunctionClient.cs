namespace Gabo.AzureFunctionsTelemetryAppInsightsConnectionStringIntegrationTests.TestInfrastructure;

internal abstract class TelemetryFunctionClient : IDisposable
{
    private readonly HttpClient _httpClient;

    protected TelemetryFunctionClient(int port)
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(5);
        _httpClient.BaseAddress = new Uri($"http://localhost:{port}/");
    }

    public Task<HttpResponseMessage> EmitCustomEventAsync()
    {
        return _httpClient.GetAsync(new Uri("event", UriKind.Relative));
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
