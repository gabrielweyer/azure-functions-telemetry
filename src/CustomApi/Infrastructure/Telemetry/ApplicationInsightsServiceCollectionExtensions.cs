using System.Linq;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CustomApi.Infrastructure.Telemetry
{
    public static class ApplicationInsightsServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationInsights(this IServiceCollection services)
        {
            var applicationDescriptor = new ApplicationDescriptor
            {
                Name = "custom-api",
                Version = "local"
            };

            services.AddSingleton(applicationDescriptor);
            services.AddSingleton<ITelemetryInitializer, ApplicationInitializer>();

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

                        newConfig.TelemetryProcessorChainBuilder.Use(next => new SomeSortOfFilter(next));
                        newConfig.TelemetryProcessorChainBuilder.Use(next => new FunctionExecutionTracesFilter(next));
                        newConfig.TelemetryProcessorChainBuilder.Use(next => new DuplicateExceptionsFilter(next));

                        foreach (var processor in telemetryConfiguration.TelemetryProcessors)
                        {
                            newConfig.TelemetryProcessorChainBuilder.Use(next => processor);

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
        private static void AddNoOpTelemetryConfigurationIfOneIsNotPresent(this IServiceCollection services)
        {
            services.TryAddSingleton(new TelemetryConfiguration());
        }
    }
}