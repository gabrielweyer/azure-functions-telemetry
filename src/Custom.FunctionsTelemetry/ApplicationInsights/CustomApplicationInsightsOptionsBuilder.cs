using System;
using System.Collections.Generic;

namespace Custom.FunctionsTelemetry.ApplicationInsights
{
    public class CustomApplicationInsightsOptionsBuilder
    {
        private readonly string _applicationName;
        private readonly Type _typeFromEntryAssembly;
        private bool _hasServiceBusTriggerFilter;
        private List<string> _serviceBusTriggeredFunctionNames;
        private string _healthCheckFunctionName;

        /// <summary>
        /// Helps you configure Application Insights.
        /// </summary>
        /// <param name="applicationName">Will be used as the 'Cloud role name'.</param>
        /// <param name="typeFromEntryAssembly">The `AssemblyInformationalVersion` of the assembly will be used as the
        /// 'Application version'.</param>
        public CustomApplicationInsightsOptionsBuilder(string applicationName, Type typeFromEntryAssembly)
        {
            _applicationName = applicationName;
            _typeFromEntryAssembly = typeFromEntryAssembly;
            _hasServiceBusTriggerFilter = false;
            _serviceBusTriggeredFunctionNames = new List<string>();
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
        /// Exceptions are recorded twice for the HTTP binding and three times for the Service Bus binding. The second
        /// duplicate exception can easily be eliminated for all bindings. If we want to discard the third duplicate
        /// exception we have to do so using the Function name
        /// (e.g. <code>[FunctionName("AppendActionConsumer")]</code>).
        ///
        /// If you do not call this function or forget to provide one of the Service Bus triggered Function name, you'll
        /// end up with duplicate exceptions.
        /// </summary>
        /// <param name="functionNames"></param>
        /// <returns></returns>
        [Obsolete("The library now attempts to determine the Service Bus triggered Functions. This remains available as an escape hatch to allow you to override the list of Functions if you're not satisfied with the result. Ultimately this setting will be removed.")]
        public CustomApplicationInsightsOptionsBuilder DiscardServiceBusDuplicateExceptions(List<string> functionNames)
        {
            _serviceBusTriggeredFunctionNames = functionNames;

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
            _healthCheckFunctionName = healthCheckFunctionName;

            return this;
        }

        public CustomApplicationInsightsOptions Build() =>
            new CustomApplicationInsightsOptions()
            {
                ApplicationName = _applicationName,
                TypeFromEntryAssembly = _typeFromEntryAssembly,
                HasServiceBusTriggerFilter = _hasServiceBusTriggerFilter,
#pragma warning disable CS0618 // Even though it's obsolete, we still need to support it!
                ServiceBusTriggeredFunctionNames = _serviceBusTriggeredFunctionNames,
#pragma warning restore CS0618
                HealthCheckFunctionName = _healthCheckFunctionName,
            };
    }
}