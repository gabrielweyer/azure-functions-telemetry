namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

public class CustomApplicationInsightsOptions
{
    public string ApplicationName { get; }
    public Type TypeFromEntryAssembly { get; }
    public bool HasServiceBusTriggerFilter { get; }
    [Obsolete("The library now attempts to determine the Service Bus triggered Functions. This remains available as an escape hatch to allow you to override the list of Functions if you're not satisfied with the result. Ultimately this setting will be removed.")]
    public List<string> ServiceBusTriggeredFunctionNames { get; }
    public string? HealthCheckFunctionName { get; }

    public CustomApplicationInsightsOptions(
        string applicationName,
        Type typeFromEntryAssembly,
        bool hasServiceBusTriggerFilter = false,
        string? healthCheckFunctionName = null,
        List<string>? serviceBusTriggeredFunctionNames = null)
    {
        if (healthCheckFunctionName != null && string.IsNullOrWhiteSpace(healthCheckFunctionName))
        {
            throw new ArgumentOutOfRangeException(
                nameof(healthCheckFunctionName),
                healthCheckFunctionName,
                "When the health check Function Name is provided, it should not be empty or consist only of white-space characters.");
        }

        ApplicationName = applicationName;
        TypeFromEntryAssembly = typeFromEntryAssembly;
        HasServiceBusTriggerFilter = hasServiceBusTriggerFilter;
        HealthCheckFunctionName = healthCheckFunctionName;
#pragma warning disable CS0618 // Even though it's obsolete, we still need to support it!
        ServiceBusTriggeredFunctionNames = serviceBusTriggeredFunctionNames ?? new List<string>();
#pragma warning restore CS0618
    }
}
