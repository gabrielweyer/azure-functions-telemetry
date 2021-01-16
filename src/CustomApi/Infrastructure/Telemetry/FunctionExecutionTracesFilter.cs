using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace CustomApi.Infrastructure.Telemetry
{
    public class FunctionExecutionTracesFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public FunctionExecutionTracesFilter(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is TraceTelemetry trace)
            {
                if (TelemetryHelper.IsFunctionStartedTelemetry(trace))
                {
                    return;
                }

                if (TelemetryHelper.IsFunctionCompletedTelemetry(trace))
                {
                    return;
                }

                if (IsServiceBusBindingMessageErrorProcessingTrace(trace))
                {
                    return;
                }
            }

            _next.Process(item);
        }

        private static bool IsServiceBusBindingMessageErrorProcessingTrace(TraceTelemetry trace)
        {
            return trace.SeverityLevel == SeverityLevel.Error &&
                   TelemetryHelper.TryGetCategory(trace, out var category) &&
                   StringHelper.IsSame(FunctionRuntimeCategory.HostExecutor, category) &&
                   !string.IsNullOrEmpty(trace.Message) &&
                   trace.Message.StartsWith("Message processing error", StringComparison.OrdinalIgnoreCase);
        }
    }
}