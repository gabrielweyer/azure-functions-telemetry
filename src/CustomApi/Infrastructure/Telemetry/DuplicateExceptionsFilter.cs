using System;
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
                if (exceptionTelemetry.Properties.TryGetValue("Category", out var category) &&
                    IsSame(CategoryUsedByAllBindings, category))
                {
                    return;
                }
            }

            _next.Process(item);
        }

        private static bool IsSame(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        private const string CategoryUsedByAllBindings = "Host.Results";
    }
}