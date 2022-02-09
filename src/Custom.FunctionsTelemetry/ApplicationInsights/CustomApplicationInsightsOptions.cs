using System;
using System.Collections.Generic;

namespace Custom.FunctionsTelemetry.ApplicationInsights
{
    public class CustomApplicationInsightsOptions
    {
        public string ApplicationName { get; set; }
        public Type TypeFromEntryAssembly { get; set; }
        public bool HasServiceBusRequestInitializer { get; set; }
        public bool HasServiceBusTriggerFilter { get; set; }
        public List<string> ServiceBusTriggeredFunctionNames { get; set; }
        public string HealthCheckFunctionName { get; set; }
        public string DependencyTypeToFilter { get; set; }
    }
}