using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace CustomApi.Infrastructure.Telemetry
{
    public class FunctionExecutionTracesFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        private const string EventIdPropertyKey = "EventId";
        private const string EventNamePropertyKey = "EventName";
        private const string CategoryPropertyKey = "Category";

        public FunctionExecutionTracesFilter(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is TraceTelemetry trace)
            {
                if (IsFunctionStartedTrace(trace))
                {
                    return;
                }

                if (IsFunctionCompletedTrace(trace))
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

        private static bool IsFunctionStartedTrace(TraceTelemetry trace)
        {
            return HasFunctionStartedEventId(trace) && HasFunctionStartedEventName(trace);
        }

        private static bool HasFunctionStartedEventId(TraceTelemetry trace)
        {
            return trace.Properties.TryGetValue(EventIdPropertyKey, out var eventId) &&
                   IsSame(FunctionRuntimeEventId.FunctionStarted, eventId);
        }

        private static bool HasFunctionStartedEventName(TraceTelemetry trace)
        {
            return trace.Properties.TryGetValue(EventNamePropertyKey, out var eventName) &&
                   IsSame(FunctionRuntimeEventName.FunctionStarted, eventName);
        }

        private static bool IsFunctionCompletedTrace(TraceTelemetry trace)
        {
            return HasFunctionCompletedEventId(trace) && HasFunctionCompletedEventName(trace);
        }

        private static bool IsServiceBusBindingMessageErrorProcessingTrace(TraceTelemetry trace)
        {
            return trace.SeverityLevel == SeverityLevel.Error &&
                   IsCategory(trace, "Host.Executor") &&
                   !string.IsNullOrEmpty(trace.Message) &&
                   trace.Message.StartsWith("Message processing error", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCategory(TraceTelemetry trace, string expectedCategory)
        {
            return trace.Properties.TryGetValue(CategoryPropertyKey, out var actualCategory) &&
                   IsSame(expectedCategory, actualCategory);
        }

        private static bool HasFunctionCompletedEventId(TraceTelemetry trace)
        {
            return trace.Properties.TryGetValue(EventIdPropertyKey, out var eventId) &&
                   (
                       IsSame(FunctionRuntimeEventId.FunctionCompletedSucceeded, eventId) ||
                       IsSame(FunctionRuntimeEventId.FunctionCompletedFailed, eventId)
                   );
        }

        private static bool HasFunctionCompletedEventName(TraceTelemetry trace)
        {
            return trace.Properties.TryGetValue(EventNamePropertyKey, out var eventName) &&
                   IsSame(FunctionRuntimeEventName.FunctionCompleted, eventName);
        }

        private static bool IsSame(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        private static class FunctionRuntimeEventId
        {
            public const string FunctionStarted = "1";
            public const string FunctionCompletedSucceeded = "2";
            public const string FunctionCompletedFailed = "3";
        }

        private static class FunctionRuntimeEventName
        {
            public const string FunctionStarted = "FunctionStarted";
            public const string FunctionCompleted = "FunctionCompleted";
        }
    }
}