namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

/// <summary>
/// Used to configure the Application Insights integration. Not intended to be used directly, it's recommended to
/// rely on <see cref="CustomApplicationInsightsConfigBuilder"/> to construct it.
/// </summary>
public class CustomApplicationInsightsConfig
{
    /// <summary>
    /// Will be used as the 'Cloud role name'.
    /// </summary>
    public string? ApplicationName { get; }
    /// <summary>
    /// The 'AssemblyInformationalVersion' of the assembly will be used as the 'Application version'. Falls back to
    /// 'unknown'.
    /// </summary>
    public Type TypeFromEntryAssembly { get; }
    /// <summary>
    /// The configuration section where we'll read the <see cref="CustomApplicationInsightsOptions"/> from.
    /// </summary>
    public string ConfigurationSectionName { get; }

    /// <summary>
    /// Even though you can call this constructor yourself, we provide
    /// <see cref="CustomApplicationInsightsConfigBuilder"/> to make it easier to configure the integration.
    /// </summary>
    /// <param name="applicationName">Will be used as the 'Cloud role name'. If not provided, we won't set the
    /// 'Cloud role name' and we will not set the 'Application version'.</param>
    /// <param name="typeFromEntryAssembly">The 'AssemblyInformationalVersion' of the assembly will be used as the
    /// 'Application version'. Falls back to 'unknown'. If applicationName is not provided, we will not set the
    /// 'Application version'. We also use this type to discover Service Bus triggered Functions so that we can apply
    /// special handling to them.</param>
    /// <param name="configurationSectionName">The configuration section where we'll read the
    /// <see cref="CustomApplicationInsightsOptions"/> from.</param>
    /// <exception cref="ArgumentOutOfRangeException">The configuration section name should not be empty or consist only
    /// of white-space characters.</exception>
    public CustomApplicationInsightsConfig(
        string? applicationName,
        Type typeFromEntryAssembly,
        string configurationSectionName)
    {
        if (string.IsNullOrWhiteSpace(configurationSectionName))
        {
            throw new ArgumentOutOfRangeException(
                nameof(configurationSectionName),
                configurationSectionName,
                "The configuration section name should not be empty or consist only of white-space characters.");
        }

        ApplicationName = applicationName;
        TypeFromEntryAssembly = typeFromEntryAssembly;
        ConfigurationSectionName = configurationSectionName;
    }
}
