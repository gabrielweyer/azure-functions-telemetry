namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

internal class FunctionExecutionTracesFilter : ITelemetryProcessor
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
        if (trace.SeverityLevel != SeverityLevel.Error)
        {
            return false;
        }

        if (!TelemetryHelper.TryGetCategory(trace, out var category) || category == null)
        {
            return false;
        }

        return StringHelper.IsSame(FunctionRuntimeCategory.ServiceBusListener, category) &&
               !string.IsNullOrEmpty(trace.Message) &&
               trace.Message.StartsWith("Message processing error", StringComparison.OrdinalIgnoreCase);
    }
}
