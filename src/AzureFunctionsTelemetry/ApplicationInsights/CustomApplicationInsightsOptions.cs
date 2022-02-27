namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights
{
    public class CustomApplicationInsightsOptions
    {
        public string ApplicationName { get; set; }
        public Type TypeFromEntryAssembly { get; set; }
        public bool HasServiceBusTriggerFilter { get; set; }
        [Obsolete("The library now attempts to determine the Service Bus triggered Functions. This remains available as an escape hatch to allow you to override the list of Functions if you're not satisfied with the result. Ultimately this setting will be removed.")]
        public List<string> ServiceBusTriggeredFunctionNames { get; set; }
        public string HealthCheckFunctionName { get; set; }
    }
}