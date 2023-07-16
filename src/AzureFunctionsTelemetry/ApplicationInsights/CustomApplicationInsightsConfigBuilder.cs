using Microsoft.Azure.WebJobs;

namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

/// <summary>
/// Provided as a convenience to make it easier to create <see cref="CustomApplicationInsightsConfig"/>.
/// </summary>
public class CustomApplicationInsightsConfigBuilder
{
    private readonly string _applicationName;
    private readonly Type _typeFromEntryAssembly;
    private string _configurationSectionName = "ApplicationInsights";

    /// <summary>
    /// <para>Helps you configure Application Insights.</para>
    /// <para>By default we'll read the <see cref="CustomApplicationInsightsOptions"/> from the 'ApplicationInsights'
    /// configuration section.</para>
    /// </summary>
    /// <param name="applicationName">Will be used as the 'Cloud role name'.</param>
    /// <param name="typeFromEntryAssembly">The 'AssemblyInformationalVersion' of the assembly will be used as the
    /// 'Application version'. Falls back to 'unknown'.</param>
    public CustomApplicationInsightsConfigBuilder(string applicationName, Type typeFromEntryAssembly)
    {
        _applicationName = applicationName;
        _typeFromEntryAssembly = typeFromEntryAssembly;
    }

    /// <summary>
    /// Allows you to set the configuration section where we'll read the <see cref="CustomApplicationInsightsOptions"/>
    /// from. The default value is 'ApplicationInsights'.
    /// </summary>
    /// <param name="configurationSectionName">The configuration section that will be used to read the options. If you
    /// don't call this method, we default to 'ApplicationInsights'.</param>
    /// <returns><see cref="CustomApplicationInsightsConfigBuilder"/> with an updated configuration section
    /// name.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The configuration section name should not be empty or consist only
    /// of white-space characters.</exception>
    public CustomApplicationInsightsConfigBuilder WithConfigurationSectionName(string configurationSectionName)
    {
        if (string.IsNullOrWhiteSpace(configurationSectionName))
        {
            throw new ArgumentOutOfRangeException(
                nameof(configurationSectionName),
                configurationSectionName,
                "The configuration section name should not be empty or consist only of white-space characters.");
        }

        _configurationSectionName = configurationSectionName;

        return this;
    }

    /// <summary>
    /// Recommended on high-traffic services. This will discard the trace telemetry with the details of the Service Bus
    /// trigger.
    /// </summary>
    /// <returns><see cref="CustomApplicationInsightsConfigBuilder"/> with Service Bus trigger filter enabled.</returns>
    [Obsolete("This has been replaced by CustomApplicationInsightsOptions. See migration guide: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/docs/Migrate-from-v1-to-v2.md.", true)]
    public CustomApplicationInsightsConfigBuilder WithServiceBusTriggerFilter()
    {
        throw new NotSupportedException("This has been replaced by CustomApplicationInsightsOptions. See migration guide: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/docs/Migrate-from-v1-to-v2.md.");
    }

    /// <summary>
    /// We will discard all '200' HTTP status code requests for the specified Function. We only support a single health
    /// check function per Function App. Calling this method multiple times will replace the previous value.
    /// </summary>
    /// <param name="healthCheckFunctionName">Supply the Function name (the argument provided to the
    /// <see cref="FunctionNameAttribute"/>)</param>
    /// <returns><see cref="CustomApplicationInsightsConfigBuilder"/> with the health check Function name
    /// configured.</returns>
    [Obsolete("This has been replaced by CustomApplicationInsightsOptions. See migration guide: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/docs/Migrate-from-v1-to-v2.md.", true)]
    public CustomApplicationInsightsConfigBuilder WithHealthRequestFilter(string healthCheckFunctionName)
    {
        throw new NotSupportedException("This has been replaced by CustomApplicationInsightsOptions. See migration guide: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/docs/Migrate-from-v1-to-v2.md.");
    }

    /// <summary>
    /// Once you're done configuring the integration, call this method to build the
    /// <see cref="CustomApplicationInsightsConfig"/>.
    /// </summary>
    /// <returns>The configured <see cref="CustomApplicationInsightsConfig"/>.</returns>
    public CustomApplicationInsightsConfig Build() =>
        new(_applicationName, _typeFromEntryAssembly, _configurationSectionName);
}
