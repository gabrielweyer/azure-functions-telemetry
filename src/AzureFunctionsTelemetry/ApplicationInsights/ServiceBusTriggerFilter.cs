namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

internal class ServiceBusTriggerFilter : ITelemetryProcessor
{
    // This string literal is kept as a single-line so that it can be searched
    private const string ServiceBusTriggerLogTemplate =
        "Trigger Details: MessageId: {MessageId}, SequenceNumber: {SequenceNumber}, DeliveryCount: {DeliveryCount}, EnqueuedTimeUtc: {EnqueuedTimeUtc}, LockedUntilUtc: {LockedUntilUtc}, SessionId: {SessionId}";

    private readonly ITelemetryProcessor _next;

    public ServiceBusTriggerFilter(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        if (item is TraceTelemetry trace && HasTriggerDetailsTemplate(trace))
        {
            return;
        }

        _next.Process(item);
    }

    private static bool HasTriggerDetailsTemplate(TraceTelemetry trace) =>
        trace.Properties.TryGetValue("prop__{OriginalFormat}", out var template) &&
        StringHelper.IsSame(ServiceBusTriggerLogTemplate, template);
}
