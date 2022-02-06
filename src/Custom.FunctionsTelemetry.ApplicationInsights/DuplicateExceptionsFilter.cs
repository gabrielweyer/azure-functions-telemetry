using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Custom.FunctionsTelemetry.ApplicationInsights
{
    internal class DuplicateExceptionsFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;
        private readonly List<string> _serviceBusFunctionCategories;

        public DuplicateExceptionsFilter(ITelemetryProcessor next, List<string> serviceBusTriggeredFunctionNames)
        {
            _next = next;
            _serviceBusFunctionCategories =
                serviceBusTriggeredFunctionNames.Select(name => $"Function.{name}").ToList();
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

                    if (_serviceBusFunctionCategories.Count > 0 &&
                        _serviceBusFunctionCategories.Contains(category) &&
                        TelemetryHelper.IsFunctionCompletedTelemetry(exceptionTelemetry))
                    {
                        return;
                    }
                }
            }

            _next.Process(item);
        }
    }
}