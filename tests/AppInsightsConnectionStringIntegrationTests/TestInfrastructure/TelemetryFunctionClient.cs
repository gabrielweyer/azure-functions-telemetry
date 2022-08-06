namespace Gabo.AzureFunctionsTelemetryAppInsightsConnectionStringIntegrationTests.TestInfrastructure;

internal abstract class TelemetryFunctionClient
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
        return _httpClient.GetAsync("event");
    }
}

internal class CustomTelemetryFunctionClient : TelemetryFunctionClient
{
    public CustomTelemetryFunctionClient() : base(7074)
    {
    }
}

internal class DefaultTelemetryFunctionClient : TelemetryFunctionClient
{
    public DefaultTelemetryFunctionClient() : base(7073)
    {
    }
}