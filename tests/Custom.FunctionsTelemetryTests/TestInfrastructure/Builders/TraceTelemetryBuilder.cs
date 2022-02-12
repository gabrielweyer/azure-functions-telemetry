using Custom.FunctionsTelemetry;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs.Logging;

namespace Custom.FunctionsTelemetryTests.TestInfrastructure.Builders;

public class TraceTelemetryBuilder
{
    private readonly string _category;
    private string _eventId;
    private string _eventName;
    private SeverityLevel? _severityLevel;
    private string _message;

    public TraceTelemetryBuilder(string category)
    {
        _category = category;
    }

    private TraceTelemetryBuilder WithEventId(string eventId)
    {
        _eventId = eventId;
        return this;
    }

    private TraceTelemetryBuilder WithEventName(string eventName)
    {
        _eventName = eventName;
        return this;
    }

    public TraceTelemetryBuilder WithSeverityLevel(SeverityLevel severityLevel)
    {
        _severityLevel = severityLevel;
        return this;
    }

    public TraceTelemetryBuilder WithMessage(string message)
    {
        _message = message;
        return this;
    }

    public static TraceTelemetry AsFunctionStarted()
    {
        return new TraceTelemetryBuilder(null)
            .WithEventId(FunctionRuntimeEventId.FunctionStarted)
            .WithEventName(FunctionRuntimeEventName.FunctionStarted)
            .Build();
    }

    public static TraceTelemetry AsFunctionCompletedSucceeded()
    {
        return new TraceTelemetryBuilder(null)
            .WithEventId(FunctionRuntimeEventId.FunctionCompletedSucceeded)
            .WithEventName(FunctionRuntimeEventName.FunctionCompleted)
            .Build();
    }

    public static TraceTelemetry AsFunctionCompletedFailed()
    {
        return new TraceTelemetryBuilder(null)
            .WithEventId(FunctionRuntimeEventId.FunctionCompletedFailed)
            .WithEventName(FunctionRuntimeEventName.FunctionCompleted)
            .Build();
    }

    public static TraceTelemetry AsServiceBusTrigger()
    {
        var traceTelemetry = new TraceTelemetryBuilder(null).Build();
        traceTelemetry.Properties["prop__{OriginalFormat}"] = "Trigger Details: MessageId: {MessageId}, SequenceNumber: {SequenceNumber}, DeliveryCount: {DeliveryCount}, EnqueuedTimeUtc: {EnqueuedTimeUtc}, LockedUntilUtc: {LockedUntilUtc}, SessionId: {SessionId}";
        return traceTelemetry;
    }

    public TraceTelemetry Build()
    {
        var traceTelemetry = new TraceTelemetry();
        traceTelemetry.Properties[LogConstants.CategoryNameKey] = _category;
        traceTelemetry.Properties[LogConstants.EventIdKey] = _eventId;
        traceTelemetry.Properties[LogConstants.EventNameKey] = _eventName;
        traceTelemetry.SeverityLevel = _severityLevel;
        traceTelemetry.Message = _message;
        return traceTelemetry;
    }
}