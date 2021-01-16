using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace CustomApi.Infrastructure.Telemetry
{
    public class DuplicateExceptionsFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public DuplicateExceptionsFilter(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is ExceptionTelemetry exceptionTelemetry)
            {
                if (TelemetryHelper.TryGetCategory(exceptionTelemetry, out var category))
                {
                    if (StringHelper.IsSame(FunctionRuntimeCategory.HostResults, category))
                    {
                        return;
                    }

                    if (ServiceBusFunctionCategories.Contains(category) &&
                        TelemetryHelper.IsFunctionCompletedTelemetry(exceptionTelemetry))
                    {
                        return;
                    }
                }
            }

            _next.Process(item);
        }

        private static readonly List<string> ServiceBusFunctionCategories = new List<string>
        {
            "Function.QueueFunction"
        };
    }
}