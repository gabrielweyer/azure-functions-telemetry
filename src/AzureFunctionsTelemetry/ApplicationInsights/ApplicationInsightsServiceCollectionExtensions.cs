using System.Diagnostics;
using System.Reflection;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

/// <summary>
/// This won't actually be displayed
/// </summary>
public static class ApplicationInsightsServiceCollectionExtensions
{
    /// <summary>
    /// Extension method taking over the Application Insights' configuration so that we can register our own Telemetry
    /// Initializers and Processors. Stamps each telemetry item with the application name and version. Discards as much
    /// redundant telemetry as is humanely possible.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> holding all your precious types.</param>
    /// <param name="options">The <see cref="CustomApplicationInsightsOptions"/> configuring the Application Insights
    /// integration.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance but with modified registrations.</returns>
    public static IServiceCollection AddCustomApplicationInsights(
        this IServiceCollection services,
        CustomApplicationInsightsOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var applicationVersion = GetAssemblyInformationalVersion(options.TypeFromEntryAssembly);
        var applicationDescriptor = new ApplicationDescriptor(options.ApplicationName, applicationVersion);
        services.AddSingleton(applicationDescriptor);
        services.AddSingleton<ITelemetryInitializer, ApplicationInitializer>();

        var serviceBusTriggeredFunctionName = FunctionsFinder
                .GetServiceBusTriggeredFunctionNames(options.TypeFromEntryAssembly);

        if (serviceBusTriggeredFunctionName.Any())
        {
            services.AddSingleton<ITelemetryInitializer, ServiceBusRequestInitializer>();
        }

        /*
         * When adding a telemetry processor using AddApplicationInsightsTelemetryProcessor, the telemetry processor
         * was not being called. Research yielded the below GitHub issue:
         *
         * https://github.com/Azure/azure-functions-host/issues/3741#issuecomment-548463346
         *
         * What follows in an improvement on the workaround suggested in the thread.
         */
        var configDescriptor = services.SingleOrDefault(sd => sd.ServiceType == typeof(TelemetryConfiguration));

        if (configDescriptor?.ImplementationFactory == null)
        {
            services.AddNoOpTelemetryConfigurationIfOneIsNotPresent();
            return services;
        }

        var implementationFactory = configDescriptor.ImplementationFactory;
        services.Remove(configDescriptor);
        services.AddSingleton(serviceProvider =>
        {
            if (implementationFactory.Invoke(serviceProvider) is not TelemetryConfiguration registeredConfig)
            {
                throw new InvalidOperationException(
                    "The service descriptor implementation factory did not return a 'TelemetryConfiguration' instance.");
            }

            var replacementConfig = new TelemetryConfiguration(
                registeredConfig.InstrumentationKey,
                registeredConfig.TelemetryChannel)
            {
                ApplicationIdProvider = registeredConfig.ApplicationIdProvider
            };

            registeredConfig.TelemetryInitializers.ToList()
                .ForEach(initializer => replacementConfig.TelemetryInitializers.Add(initializer));

            /*
             * By default the SDK registers the below telemetry processors (in order):
             *
             * 1. OperationFilteringTelemetryProcessor
             * 2. QuickPulseTelemetryProcessor
             * 3. FilteringTelemetryProcessor
             * 4. AdaptiveSamplingTelemetryProcessor
             * 5. PassThroughProcessor
             *
             * When we invoked the above TelemetryConfiguration factory, one of the side-effect was to
             * instantiate an instance of HostingDiagnosticListener. HostingDiagnosticListener is
             * responsible for tracking the requests in Azure Functions. As HostingDiagnosticListener was
             * provided with an instance of TelemetryClient, it will have no knowledge of the additional
             * telemetry processors we're adding now and our telemetry processors will not be executed for
             * the RequestTelemetry items.
             *
             * One solution to this problem is to insert our telemetry processors between
             * OperationFilteringTelemetryProcessor and QuickPulseTelemetryProcessor.
             */

            /*
             * Both `OperationFilteringTelemetryProcessor` and `PassThroughProcessor` are internal.
             * They're coming from two different assemblies. Somehow in .NET Core 3.1 I can get both
             * types from their assembly using `GetType("{FullName}") but in .NET 6 I can
             * only get the the `PassThroughProcessor`. Instead I decided to match on type name.
             */
            const string passThroughProcessorTypeFullName =
                "Microsoft.ApplicationInsights.Shared.Extensibility.Implementation.PassThroughProcessor";
            const string operationFilteringTelemetryProcessorTypeFullName =
                "Microsoft.Azure.WebJobs.Logging.ApplicationInsights.OperationFilteringTelemetryProcessor";

            foreach (var processor in registeredConfig.TelemetryProcessors)
            {
                var processorType = processor.GetType();
                var processorTypeName = processorType.FullName;

                if (passThroughProcessorTypeFullName.Equals(processorTypeName, StringComparison.Ordinal))
                {
                    /*
                     * The current TelemetryProcessorChainBuilder and the new one we're building both have a
                     * PassThroughProcessor so we end up with two of them in the final chain.
                     *
                     * It doesn't seem to be causing an issue but I'd rather have only one of them, dropping
                     * the current one.
                     */
                    continue;
                }

                if (operationFilteringTelemetryProcessorTypeFullName.Equals(processorTypeName, StringComparison.Ordinal))
                {
                    var operationFilteringProcessorNextField = processorType
                        .GetField("_next", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (operationFilteringProcessorNextField == null)
                    {
                        throw new InvalidOperationException(
                            "We expect 'OperationFilteringTelemetryProcessor' to have a private field named '_next'.");
                    }

                    if (operationFilteringProcessorNextField.GetValue(processor) is not ITelemetryProcessor
                        quickPulseTelemetryProcessor)
                    {
                        throw new InvalidOperationException(
                            "The 'OperationFilteringTelemetryProcessor' '_next' field is not pointing to a Telemetry Processor.");
                    }

                    /*
                     * We now instantiate our processors in the reverse order so that the last one can point
                     * to `QuickPulseTelemetryProcessor` and `OperationFilteringTelemetryProcessor` can
                     * point to the first one.
                     */
                    var customProcessors = new List<ITelemetryProcessor>();

                    var duplicateExceptionFilter = new DuplicateExceptionsFilter(
                        quickPulseTelemetryProcessor,
                        serviceBusTriggeredFunctionName);
                    customProcessors.Insert(0, duplicateExceptionFilter);
                    var functionExecutionTracesFilter = new FunctionExecutionTracesFilter(customProcessors.First());
                    customProcessors.Insert(0, functionExecutionTracesFilter);

                    if (options.HasServiceBusTriggerFilter)
                    {
                        var serviceBusTriggerFilter = new ServiceBusTriggerFilter(customProcessors.First());
                        customProcessors.Insert(0, serviceBusTriggerFilter);
                    }

                    if (options.HealthCheckFunctionName != null)
                    {
                        var healthRequestFilter = new HealthRequestFilter(
                            customProcessors.First(),
                            options.HealthCheckFunctionName);
                        customProcessors.Insert(0, healthRequestFilter);
                    }

                    var processorFactoryServiceDescriptors = services
                        .Where(sd => sd.ServiceType == typeof(ITelemetryProcessorFactory))
                        .ToList();

                    foreach (var processorFactoryServiceDescriptor in processorFactoryServiceDescriptors)
                    {
                        if (processorFactoryServiceDescriptor.ImplementationFactory == null)
                        {
                            throw new InvalidOperationException(
                                $"The telemetry processor factory implementation factory for '{processorFactoryServiceDescriptor.ServiceType}' is null.");
                        }

                        if (processorFactoryServiceDescriptor.ImplementationFactory(serviceProvider) is not
                            ITelemetryProcessorFactory processorFactory)
                        {
                            throw new InvalidOperationException(
                                "The telemetry processor factory implementation factory for '{processorFactoryServiceDescriptor.ServiceType}' did not return a 'ITelemetryProcessorFactory' instance.");
                        }

                        var processorAddedThroughDi = processorFactory.Create(customProcessors.First());
                        customProcessors.Insert(0, processorAddedThroughDi);
                    }

                    replacementConfig.TelemetryProcessorChainBuilder.Use(_ => processor);
                    operationFilteringProcessorNextField.SetValue(processor, customProcessors.First());

                    foreach (var customProcessor in customProcessors)
                    {
                        replacementConfig.TelemetryProcessorChainBuilder.Use(_ => customProcessor);
                    }
                }
                else
                {
                    replacementConfig.TelemetryProcessorChainBuilder.Use(_ => processor);
                }

                if (processor is QuickPulseTelemetryProcessor quickPulseProcessor)
                {
                    var quickPulseModule = new QuickPulseTelemetryModule();
                    quickPulseModule.RegisterTelemetryProcessor(quickPulseProcessor);
                }
            }

            replacementConfig.TelemetryProcessorChainBuilder.Build();
            replacementConfig.TelemetryProcessors.OfType<ITelemetryModule>().ToList()
                .ForEach(module => module.Initialize(replacementConfig));
            return replacementConfig;
        });

        return services;
    }

    /// <summary>
    /// When the configuration APPINSIGHTS_INSTRUMENTATIONKEY / APPLICATIONINSIGHTS_CONNECTION_STRING key is not present, the Functions runtime does not
    /// register Application Insights with the Inversion of Control container:
    ///
    /// https://github.com/Azure/azure-functions-host/blob/7d9cd7fc69b282b2f6ce2c2ee1574a236bc69202/src/WebJobs.Script/ScriptHostBuilderExtensions.cs#L368-L372
    ///
    /// On the other hand Azure Functions documentation recommends to inject TelemetryConfiguration:
    ///
    /// https://docs.microsoft.com/en-us/azure/azure-functions/functions-monitoring?tabs=cmd#log-custom-telemetry-in-c-functions
    ///
    /// So if we don't configure the instrumentation key / connection string we end up with a runtime exception when serving the
    /// Function:
    ///
    /// Microsoft.Extensions.DependencyInjection.Abstractions: Unable to resolve service for type 'Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration' while attempting to activate [...].
    ///
    /// It's common to run locally without having Application Insights configured. Forcing the developer to
    /// configure a "fake" instrumentation key / connection string will get the SDK to send telemetry even if the key is invalid. A
    /// better solution is to register a no-op TelemetryConfiguration instance. As our registration runs after the
    /// runtime has configured Application Insights, our no-op TelemetryConfiguration will only be registered if one
    /// wasn't registered previously.
    /// </summary>
    /// <param name="services"></param>
    private static void AddNoOpTelemetryConfigurationIfOneIsNotPresent(this IServiceCollection services)
    {
#pragma warning disable CA2000
        /* This instance will not be disposed on shutdown. That's not great but this instance is only registered when
         * an instrumentation key / connection string is not present. As far as I know Azure Functions don't support
         * IApplicationLifetime or hosted services.
         */
        services.TryAddSingleton(new TelemetryConfiguration());
#pragma warning restore
    }

    private static string GetAssemblyInformationalVersion(Type type)
    {
        var productVersion = FileVersionInfo.GetVersionInfo(type.Assembly.Location).ProductVersion;

        return string.IsNullOrWhiteSpace(productVersion) ? "unknown" : productVersion;
    }
}
