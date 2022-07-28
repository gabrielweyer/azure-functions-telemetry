namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

public class CustomApplicationInsightsOptions
{
    public string ApplicationName { get; }
    public Type TypeFromEntryAssembly { get; }
    public bool HasServiceBusTriggerFilter { get; }
    public string? HealthCheckFunctionName { get; }

    public CustomApplicationInsightsOptions(
        string applicationName,
        Type typeFromEntryAssembly,
        bool hasServiceBusTriggerFilter = false,
        string? healthCheckFunctionName = null)
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
    }
}
