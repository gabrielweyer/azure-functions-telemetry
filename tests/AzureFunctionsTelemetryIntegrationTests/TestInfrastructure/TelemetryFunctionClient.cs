namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal abstract class TelemetryFunctionClient
{
    private readonly HttpClient _httpClient;

    protected TelemetryFunctionClient(int port)
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(5);
        _httpClient.BaseAddress = new Uri($"http://localhost:{port}/");
    }

    public async Task ThrowOnHttpBindingAsync()
    {
        var response = await _httpClient.GetAsync("http-exception");
        await EnsureFailureAsync(response);
    }

    public async Task LogVerboseAsync()
    {
        var response = await _httpClient.GetAsync("trace-log");
        await EnsureSuccessAsync(response);
    }

    public async Task CheckHealthAsync()
    {
        var response = await _httpClient.GetAsync("health");
        await EnsureSuccessAsync(response);
    }

    public async Task<List<TelemetryItem>> GetTelemetryAsync()
    {
        var response = await _httpClient.GetAsync("telemetry");
        await EnsureSuccessAsync(response);
        var responseBody = await response.Content.ReadAsStringAsync();
        var serialisedTelemetryItems = responseBody.Split(
            new[] { Environment.NewLine },
            StringSplitOptions.RemoveEmptyEntries);

        return serialisedTelemetryItems
            .Select(i => JsonConvert.DeserializeObject<TelemetryItem>(i, new TelemetryItemConverter()))
            .ToList();
    }

    public async Task DeleteTelemetryAsync()
    {
        var response = await _httpClient.DeleteAsync("telemetry");
        await EnsureSuccessAsync(response);
    }

    public async Task TriggerServiceBusAsync()
    {
        var response = await _httpClient.GetAsync("service-bus");
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            await ThrowOnUnexpectedResponseAsync(response, "success");
        }
    }

    private static async Task EnsureFailureAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            await ThrowOnUnexpectedResponseAsync(response, "failure");
        }
    }

    private static async Task ThrowOnUnexpectedResponseAsync(HttpResponseMessage response, string status)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        var exceptionMessage =
            $"Response status code does not indicate {status}: {(int)response.StatusCode} ({response.StatusCode}). Body: {responseBody}";

        throw new HttpRequestException(exceptionMessage, null, response.StatusCode);
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
