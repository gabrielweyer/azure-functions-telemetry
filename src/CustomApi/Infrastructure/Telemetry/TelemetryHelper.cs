using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs.Logging;

namespace CustomApi.Infrastructure.Telemetry
{
    public static class TelemetryHelper
    {
        public static bool TryGetCategory(ISupportProperties telemetry, out string category)
        {
            return telemetry.Properties.TryGetValue(LogConstants.CategoryNameKey, out category);
        }

        private static bool TryGetEventId(ISupportProperties telemetry, out string eventId)
        {
            return telemetry.Properties.TryGetValue(LogConstants.EventIdKey, out eventId);
        }

        private static bool TryGetEventName(ISupportProperties telemetry, out string eventName)
        {
            return telemetry.Properties.TryGetValue(LogConstants.EventNameKey, out eventName);
        }

        public static bool IsFunctionStartedTelemetry(ISupportProperties trace)
        {
            return HasFunctionStartedEventId(trace) && HasFunctionStartedEventName(trace);
        }

        private static bool HasFunctionStartedEventId(ISupportProperties telemetry)
        {
            return TryGetEventId(telemetry, out var eventId) &&
                   StringHelper.IsSame(FunctionRuntimeEventId.FunctionStarted, eventId);
        }

        private static bool HasFunctionStartedEventName(ISupportProperties telemetry)
        {
            return TryGetEventName(telemetry, out var eventName) &&
                   StringHelper.IsSame(FunctionRuntimeEventName.FunctionStarted, eventName);
        }

        public static bool IsFunctionCompletedTelemetry(ISupportProperties telemetry)
        {
            return HasFunctionCompletedEventId(telemetry) && HasFunctionCompletedEventName(telemetry);
        }

        private static bool HasFunctionCompletedEventName(ISupportProperties telemetry)
        {
            return TryGetEventName(telemetry, out var eventName) &&
                   StringHelper.IsSame(FunctionRuntimeEventName.FunctionCompleted, eventName);
        }

        private static bool HasFunctionCompletedEventId(ISupportProperties telemetry)
        {
            return TryGetEventId(telemetry, out var eventId) &&
                   (
                       StringHelper.IsSame(FunctionRuntimeEventId.FunctionCompletedSucceeded, eventId) ||
                       StringHelper.IsSame(FunctionRuntimeEventId.FunctionCompletedFailed, eventId)
                   );
        }
    }
}