namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

public class CustomApplicationInsightsOptionsBuilder
{
    private readonly string _applicationName;
    private readonly Type _typeFromEntryAssembly;
    private bool _hasServiceBusTriggerFilter;
    private string? _healthCheckFunctionName;

    /// <summary>
    /// Helps you configure Application Insights.
    /// </summary>
    /// <param name="applicationName">Will be used as the 'Cloud role name'.</param>
    /// <param name="typeFromEntryAssembly">The 'AssemblyInformationalVersion' of the assembly will be used as the
    /// 'Application version'. Falls back to 'unknown'.</param>
    public CustomApplicationInsightsOptionsBuilder(string applicationName, Type typeFromEntryAssembly)
    {
        _applicationName = applicationName;
        _typeFromEntryAssembly = typeFromEntryAssembly;
        _hasServiceBusTriggerFilter = false;
        _healthCheckFunctionName = null;
    }

    /// <summary>
    /// Recommended on high-traffic services. This will discard the trace telemetry with the details of the trigger.
    /// </summary>
    /// <returns></returns>
    public CustomApplicationInsightsOptionsBuilder WithServiceBusTriggerFilter()
    {
        _hasServiceBusTriggerFilter = true;

        return this;
    }

    /// <summary>
    /// We will discard all requests for the specified Function. We only support a single health check function per
    /// Function App. Calling this method multiple times will replace the previous value.
    /// </summary>
    /// <param name="healthCheckFunctionName"></param>
    /// <returns></returns>
    public CustomApplicationInsightsOptionsBuilder WithHealthRequestFilter(string healthCheckFunctionName)
    {
        if (string.IsNullOrWhiteSpace(healthCheckFunctionName))
        {
            throw new ArgumentOutOfRangeException(
                nameof(healthCheckFunctionName),
                healthCheckFunctionName,
                "The health check Function Name should not be empty or consist only of white-space characters.");
        }

        _healthCheckFunctionName = healthCheckFunctionName;

        return this;
    }

    public CustomApplicationInsightsOptions Build() => new(
        _applicationName,
        _typeFromEntryAssembly,
        _hasServiceBusTriggerFilter,
        _healthCheckFunctionName);
}
