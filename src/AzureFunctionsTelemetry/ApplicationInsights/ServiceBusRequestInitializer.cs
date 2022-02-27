namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights
{
    internal class ServiceBusRequestInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is RequestTelemetry requestTelemetry &&
                requestTelemetry.Url == null &&
                requestTelemetry.Success.HasValue)
            {
                requestTelemetry.ResponseCode = requestTelemetry.Success.Value ? "200" : "500";
                requestTelemetry.Url = new Uri($"/{requestTelemetry.Name}", UriKind.Relative);
            }
        }
    }
}