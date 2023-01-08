namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal abstract class TelemetryFunctionClient : IDisposable
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
        var response = await _httpClient.GetAsync(new Uri("http-exception", UriKind.Relative));
        await EnsureFailureAsync(response);
    }

    public async Task LogVerboseAsync()
    {
        var response = await _httpClient.GetAsync(new Uri("trace-log", UriKind.Relative));
        await EnsureSuccessAsync(response);
    }

    public async Task CheckHealthAsync()
    {
        var response = await _httpClient.GetAsync(new Uri("health", UriKind.Relative));
        await EnsureSuccessAsync(response);
    }

    public async Task<List<TelemetryItem>> GetTelemetryAsync()
    {
        var response = await _httpClient.GetAsync(new Uri("telemetry", UriKind.Relative));
        await EnsureSuccessAsync(response);
        var responseBody = await response.Content.ReadAsStringAsync();
        var serialisedTelemetryItems = responseBody.Split(
            new[] { Environment.NewLine },
            StringSplitOptions.RemoveEmptyEntries);

        return serialisedTelemetryItems
            .Select(i => JsonConvert.DeserializeObject<TelemetryItem>(i, new TelemetryItemConverter())!)
            .ToList();
    }

    public async Task<(RequestItem item, List<TelemetryItem> allItems)> PollForTelemetryAsync(string operationName)
    {
        var attemptCount = 0;
        const int maxAttemptCount = 5;
        var delayBetweenAttempts = TimeSpan.FromMilliseconds(100);

        do
        {
            attemptCount++;
            var telemetries = await GetTelemetryAsync();
            var request = telemetries
                .Where(i => i is RequestItem)
                .Cast<RequestItem>()
                .SingleOrDefault(r => r.OperationName == operationName);

            if (request != null)
            {
                return (request, telemetries);
            }

            await Task.Delay(delayBetweenAttempts);
            delayBetweenAttempts *= 2;
        } while (attemptCount <= maxAttemptCount);

        throw new InvalidOperationException("Telemetry is still missing.");
    }

    public async Task DeleteTelemetryAsync()
    {
        var response = await _httpClient.DeleteAsync(new Uri("telemetry", UriKind.Relative));
        await EnsureSuccessAsync(response);
    }

    public async Task TriggerServiceBusAsync()
    {
        var response = await _httpClient.GetAsync(new Uri("service-bus", UriKind.Relative));
        await EnsureSuccessAsync(response);
    }

    public async Task TriggerServiceBusExceptionAsync()
    {
        var response = await _httpClient.GetAsync(new Uri("service-bus-exception", UriKind.Relative));
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

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}

internal sealed class CustomTelemetryFunctionClient : TelemetryFunctionClient
{
    public CustomTelemetryFunctionClient() : base(7074)
    {
    }
}

internal sealed class DefaultTelemetryFunctionClient : TelemetryFunctionClient
{
    public DefaultTelemetryFunctionClient() : base(7073)
    {
    }
}
