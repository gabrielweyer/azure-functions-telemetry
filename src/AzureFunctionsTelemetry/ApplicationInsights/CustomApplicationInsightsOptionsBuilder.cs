using Microsoft.Azure.WebJobs;

namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

/// <summary>
/// Provided as a convenience to make it easier to create <see cref="CustomApplicationInsightsOptions"/>.
/// </summary>
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
    /// Recommended on high-traffic services. This will discard the trace telemetry with the details of the Service Bus
    /// trigger.
    /// </summary>
    /// <returns><see cref="CustomApplicationInsightsOptionsBuilder"/> with Service Bus trigger filter
    /// enabled.</returns>
    public CustomApplicationInsightsOptionsBuilder WithServiceBusTriggerFilter()
    {
        _hasServiceBusTriggerFilter = true;

        return this;
    }

    /// <summary>
    /// We will discard all '200' HTTP status code requests for the specified Function. We only support a single health
    /// check function per Function App. Calling this method multiple times will replace the previous value.
    /// </summary>
    /// <param name="healthCheckFunctionName">Supply the Function name (the argument provided to the
    /// <see cref="FunctionNameAttribute"/>)</param>
    /// <returns><see cref="CustomApplicationInsightsOptionsBuilder"/> with the health check Function name
    /// configured.</returns>
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

    /// <summary>
    /// Once you're done configuring the integration, call this method to build the
    /// <see cref="CustomApplicationInsightsOptions"/>.
    /// </summary>
    /// <returns>The configured <see cref="CustomApplicationInsightsOptions"/>.</returns>
    public CustomApplicationInsightsOptions Build() => new(
        _applicationName,
        _typeFromEntryAssembly,
        _hasServiceBusTriggerFilter,
        _healthCheckFunctionName);
}
