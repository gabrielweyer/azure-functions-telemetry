using System;
using System.Collections.Generic;

namespace Custom.FunctionsTelemetry.ApplicationInsights
{
    public class CustomApplicationInsightsOptionsBuilder
    {
        private readonly string _applicationName;
        private readonly Type _typeFromEntryAssembly;
        private bool _hasServiceBusRequestInitializer;
        private bool _hasServiceBusTriggerFilter;
        private List<string> _serviceBusTriggeredFunctionNames;
        private string _healthCheckFunctionName;
        private string _dependencyTypeToFilter;

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
            _hasServiceBusRequestInitializer = false;
            _hasServiceBusTriggerFilter = false;
            _serviceBusTriggeredFunctionNames = new List<string>();
            _healthCheckFunctionName = null;
            _dependencyTypeToFilter = null;
        }

        /// <summary>
        /// The <see cref="ServiceBusRequestInitializer"/> sets the response code and url on Function Apps "requests"
        /// that were triggered by a Service Bus binding.
        ///
        /// These two values are left blank in the default implementation.
        /// </summary>
        /// <returns></returns>
        public CustomApplicationInsightsOptionsBuilder WithServiceBusRequestInitializer()
        {
            _hasServiceBusRequestInitializer = true;

            return this;
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
        /// end with duplicate exceptions.
        /// </summary>
        /// <param name="functionNames"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Allows you to filter out a single dependency type. Calling this method multiple times will replace the
        /// previous value.
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <returns></returns>
        public CustomApplicationInsightsOptionsBuilder WithDependencyFilter(string dependencyType)
        {
            _dependencyTypeToFilter = dependencyType;

            return this;
        }

        public CustomApplicationInsightsOptions Build() =>
            new CustomApplicationInsightsOptions()
            {
                ApplicationName = _applicationName,
                TypeFromEntryAssembly = _typeFromEntryAssembly,
                HasServiceBusRequestInitializer = _hasServiceBusRequestInitializer,
                HasServiceBusTriggerFilter = _hasServiceBusTriggerFilter,
                ServiceBusTriggeredFunctionNames = _serviceBusTriggeredFunctionNames,
                HealthCheckFunctionName = _healthCheckFunctionName,
                DependencyTypeToFilter = _dependencyTypeToFilter
            };
    }
}