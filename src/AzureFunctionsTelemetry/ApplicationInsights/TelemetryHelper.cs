using Microsoft.Azure.WebJobs.Logging;

namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

internal static class TelemetryHelper
{
    public static bool TryGetCategory(ISupportProperties telemetry, out string? category) =>
        telemetry.Properties.TryGetValue(LogConstants.CategoryNameKey, out category);

    private static bool TryGetEventId(ISupportProperties telemetry, out string? eventId) =>
        telemetry.Properties.TryGetValue(LogConstants.EventIdKey, out eventId);

    private static bool TryGetEventName(ISupportProperties telemetry, out string? eventName) =>
        telemetry.Properties.TryGetValue(LogConstants.EventNameKey, out eventName);

    public static bool IsFunctionStartedTelemetry(ISupportProperties trace) =>
        HasFunctionStartedEventId(trace) && HasFunctionStartedEventName(trace);

    private static bool HasFunctionStartedEventId(ISupportProperties telemetry)
    {
        if (!TryGetEventId(telemetry, out var eventId) || eventId == null)
        {
            return false;
        }

        return StringHelper.IsSame(FunctionRuntimeEventId.FunctionStarted, eventId);
    }

    private static bool HasFunctionStartedEventName(ISupportProperties telemetry)
    {
        if (!TryGetEventName(telemetry, out var eventName) || eventName == null)
        {
            return false;
        }

        return StringHelper.IsSame(FunctionRuntimeEventName.FunctionStarted, eventName);
    }

    public static bool IsFunctionCompletedTelemetry(ISupportProperties telemetry) =>
        HasFunctionCompletedEventId(telemetry) && HasFunctionCompletedEventName(telemetry);

    private static bool HasFunctionCompletedEventName(ISupportProperties telemetry)
    {
        if (!TryGetEventName(telemetry, out var eventName) || eventName == null)
        {
            return false;
        }

        return StringHelper.IsSame(FunctionRuntimeEventName.FunctionCompleted, eventName);
    }

    private static bool HasFunctionCompletedEventId(ISupportProperties telemetry)
    {
        if (!TryGetEventId(telemetry, out var eventId) || eventId == null)
        {
            return false;
        }

        return StringHelper.IsSame(FunctionRuntimeEventId.FunctionCompletedSucceeded, eventId) ||
               StringHelper.IsSame(FunctionRuntimeEventId.FunctionCompletedFailed, eventId);
    }
}
