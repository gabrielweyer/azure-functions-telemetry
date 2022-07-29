namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

/// <summary>
/// Used to configure the Application Insights integration. Not intended to be used directly, it's recommended to
/// rely on <see cref="CustomApplicationInsightsOptionsBuilder"/> to construct it.
/// </summary>
public class CustomApplicationInsightsOptions
{
    /// <summary>
    /// Will be used as the 'Cloud role name'.
    /// </summary>
    public string ApplicationName { get; }
    /// <summary>
    /// The 'AssemblyInformationalVersion' of the assembly will be used as the 'Application version'. Falls back to
    /// 'unknown'.
    /// </summary>
    public Type TypeFromEntryAssembly { get; }
    /// <summary>
    /// Recommended on high-traffic services. This will discard the trace telemetry with the details of the Service
    /// Bus trigger.
    /// </summary>
    public bool HasServiceBusTriggerFilter { get; }
    /// <summary>
    /// We will discard all '200' HTTP status code requests for the specified Function. We only support a single health
    /// check function per Function App, but you can create a telemetry processor to discard other requests.
    /// </summary>
    public string? HealthCheckFunctionName { get; }

    /// <summary>
    /// Even though you can call this constructor yourself, we provide
    /// <see cref="CustomApplicationInsightsOptionsBuilder"/> to make it easier to configure the integration.
    /// </summary>
    /// <param name="applicationName">Will be used as the 'Cloud role name'.</param>
    /// <param name="typeFromEntryAssembly">The 'AssemblyInformationalVersion' of the assembly will be used as the
    /// 'Application version'. Falls back to 'unknown'.</param>
    /// <param name="hasServiceBusTriggerFilter">Recommended on high-traffic services. This will discard the trace
    /// telemetry with the details of the Service Bus trigger.</param>
    /// <param name="healthCheckFunctionName">We will discard all '200' HTTP status code requests for the specified
    /// Function. If provided should have at least one character.</param>
    /// <exception cref="ArgumentOutOfRangeException">If you provide <paramref name="healthCheckFunctionName"/>, it should
    /// have at least one character.</exception>
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
