using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.Azure.WebJobs.Logging.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

[assembly: InternalsVisibleTo("Custom.FunctionsTelemetryTests")]
namespace Custom.FunctionsTelemetry.ApplicationInsights
{
    public static class ApplicationInsightsServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Telemetry Initializers and Processors.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomApplicationInsights(
            this IServiceCollection services,
            CustomApplicationInsightsOptions options)
        {
            var applicationVersion = GetAssemblyInformationalVersion(options.TypeFromEntryAssembly);
            var applicationDescriptor = new ApplicationDescriptor(options.ApplicationName, applicationVersion);
            services.AddSingleton(applicationDescriptor);
            services.AddSingleton<ITelemetryInitializer, ApplicationInitializer>();

            if (options.HasServiceBusRequestInitializer)
            {
                services.AddSingleton<ITelemetryInitializer, ServiceBusRequestInitializer>();
            }

            /*
             * When adding a telemetry processor using AddApplicationInsightsTelemetryProcessor, the telemetry processor
             * was not being called. Research yielded the below GitHub issue:
             *
             * https://github.com/Azure/azure-functions-host/issues/3741#issuecomment-548463346
             */
            var configDescriptor = services.SingleOrDefault(tc => tc.ServiceType == typeof(TelemetryConfiguration));

            if (configDescriptor?.ImplementationFactory != null)
            {
                var implFactory = configDescriptor.ImplementationFactory;
                services.Remove(configDescriptor);
                services.AddSingleton(provider =>
                {
                    if (implFactory.Invoke(provider) is TelemetryConfiguration telemetryConfiguration)
                    {
                        var newConfig = new TelemetryConfiguration(telemetryConfiguration.InstrumentationKey, telemetryConfiguration.TelemetryChannel)
                        {
                            ApplicationIdProvider = telemetryConfiguration.ApplicationIdProvider
                        };

                        telemetryConfiguration.TelemetryInitializers.ToList().ForEach(initializer => newConfig.TelemetryInitializers.Add(initializer));

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

                        // `OperationFilteringTelemetryProcessor` is an internal class
                        var operationFilteringTelemetryProcessorType = typeof(HttpAutoCollectionOptions).Assembly
                            .GetType("Microsoft.Azure.WebJobs.Logging.ApplicationInsights.OperationFilteringTelemetryProcessor");

                        // `PassThroughProcessor` is an internal class
                        var passThroughProcessorType = typeof(ITelemetryProcessor).Assembly
                            .GetType("Microsoft.ApplicationInsights.Shared.Extensibility.Implementation.PassThroughProcessor");

                        foreach (var processor in telemetryConfiguration.TelemetryProcessors)
                        {
                            if (processor.GetType() == passThroughProcessorType)
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

                            if (processor.GetType() == operationFilteringTelemetryProcessorType)
                            {
                                var operationFilteringProcessorNextField = operationFilteringTelemetryProcessorType
                                    .GetField("_next", BindingFlags.NonPublic | BindingFlags.Instance);

                                if (operationFilteringProcessorNextField == null)
                                {
                                    throw new InvalidOperationException("We expect `OperationFilteringTelemetryProcessor` to have a private field named `_next`.");
                                }

                                var quickPulseTelemetryProcessor = (ITelemetryProcessor) operationFilteringProcessorNextField
                                    .GetValue(processor);

                                /*
                                 * We now instantiate our processors in the reverse order so that the last one can point
                                 * to `QuickPulseTelemetryProcessor` and `OperationFilteringTelemetryProcessor` can
                                 * point to the first one.
                                 */
                                var customProcessors = new List<ITelemetryProcessor>();
                                var duplicateExceptionFilter = new DuplicateExceptionsFilter(
                                    quickPulseTelemetryProcessor,
                                    options.ServiceBusTriggeredFunctionNames);
                                customProcessors.Insert(0, duplicateExceptionFilter);
                                var functionExecutionTracesFilter = new FunctionExecutionTracesFilter(
                                    customProcessors.First());
                                customProcessors.Insert(0, functionExecutionTracesFilter);
                                var telemetryCounter = new TelemetryCounter(customProcessors.First());
                                customProcessors.Insert(0, telemetryCounter);

                                if (options.HasServiceBusTriggerFilter)
                                {
                                    var serviceBusTriggerFilter = new ServiceBusTriggerFilter(
                                        customProcessors.First());
                                    customProcessors.Insert(0, serviceBusTriggerFilter);
                                }

                                if (!string.IsNullOrEmpty(options.HealthCheckFunctionName))
                                {
                                    var healthRequestFilter = new HealthRequestFilter(
                                        customProcessors.First(),
                                        options.HealthCheckFunctionName);
                                    customProcessors.Insert(0, healthRequestFilter);
                                }

                                if (!string.IsNullOrEmpty(options.DependencyTypeToFilter))
                                {
                                    var dependencyFilter = new DependencyFilter(
                                        customProcessors.First(),
                                        options.DependencyTypeToFilter);
                                    customProcessors.Insert(0, dependencyFilter);
                                }

                                newConfig.TelemetryProcessorChainBuilder.Use(_ => processor);
                                operationFilteringProcessorNextField.SetValue(
                                    processor,
                                    customProcessors.First());

                                foreach (var customProcessor in customProcessors)
                                {
                                    newConfig.TelemetryProcessorChainBuilder.Use(_ => customProcessor);
                                }
                            }
                            else
                            {
                                newConfig.TelemetryProcessorChainBuilder.Use(_ => processor);
                            }

                            if (processor is QuickPulseTelemetryProcessor quickPulseProcessor)
                            {
                                var quickPulseModule = new QuickPulseTelemetryModule();
                                quickPulseModule.RegisterTelemetryProcessor(quickPulseProcessor);
                            }
                        }

                        newConfig.TelemetryProcessorChainBuilder.Build();
                        newConfig.TelemetryProcessors.OfType<ITelemetryModule>().ToList().ForEach(module => module.Initialize(newConfig));
                        return newConfig;
                    }

                    return null;
                });
            }

            services.AddNoOpTelemetryConfigurationIfOneIsNotPresent();
            return services;
        }

        /// <summary>
        /// When the configuration APPINSIGHTS_INSTRUMENTATIONKEY key is not present, the Functions runtime does not
        /// register Application Insights with the Inversion of Control container:
        ///
        /// https://github.com/Azure/azure-functions-host/blob/7d9cd7fc69b282b2f6ce2c2ee1574a236bc69202/src/WebJobs.Script/ScriptHostBuilderExtensions.cs#L368-L372
        ///
        /// On the other hand Azure Functions documentation recommends to inject TelemetryConfiguration:
        ///
        /// https://docs.microsoft.com/en-us/azure/azure-functions/functions-monitoring?tabs=cmd#log-custom-telemetry-in-c-functions
        ///
        /// So if we don't configure the instrumentation key we end up with a runtime exception when serving the
        /// Function:
        ///
        /// Microsoft.Extensions.DependencyInjection.Abstractions: Unable to resolve service for type 'Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration' while attempting to activate [...].
        ///
        /// It's common to run locally without having Application Insights configured. Forcing the developer to
        /// configure a "fake" instrumentation key will get the SDK to send telemetry even if the key is invalid. A
        /// better solution is to register a no-op TelemetryConfiguration instance. As our registration runs after the
        /// runtime has configured Application Insights, our no-op TelemetryConfiguration will only be registered if one
        /// wasn't registered previously.
        /// </summary>
        /// <param name="services"></param>
        private static IServiceCollection AddNoOpTelemetryConfigurationIfOneIsNotPresent(this IServiceCollection services)
        {
            services.TryAddSingleton(new TelemetryConfiguration());
            return services;
        }

        private static string GetAssemblyInformationalVersion(Type type)
            => FileVersionInfo.GetVersionInfo(type.Assembly.Location).ProductVersion;
    }
}