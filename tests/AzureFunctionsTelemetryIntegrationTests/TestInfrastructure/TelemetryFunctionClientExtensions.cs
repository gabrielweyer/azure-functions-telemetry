namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal static class TelemetryFunctionClientExtensions
{
    public static async Task<T> GetTelemetryItemAsync<T>(this TelemetryFunctionClient client, Func<T, bool> selector) where T : TelemetryItem
    {
        var attemptCount = 0;
        const int maxAttemptCount = 5;

        do
        {
            attemptCount++;
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            var telemetryItems = await client.GetTelemetryAsync();
            var request = telemetryItems
                .Where(i => i is T)
                .Cast<T>()
                .SingleOrDefault(selector);

            if (request != null)
            {
                return request;
            }
        } while (attemptCount < maxAttemptCount);

        throw new InvalidOperationException();
    }
}
