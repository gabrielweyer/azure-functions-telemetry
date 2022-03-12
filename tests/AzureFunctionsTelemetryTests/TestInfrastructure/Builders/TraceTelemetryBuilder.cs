using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs.Logging;

namespace Gabo.AzureFunctionsTelemetryTests.TestInfrastructure.Builders;

public class TraceTelemetryBuilder
{
    private readonly string _category;
    private readonly SeverityLevel _severityLevel;
    private readonly string _message;
    private string? _eventId;
    private string? _eventName;

    private const string DefaultFunctionName = "FunctionName";
    private const string DefaultCategory = $"Function.{DefaultFunctionName}";
    private const string DefaultInvocationId = "72343d57-51b4-4cc5-85ca-c035501df54a";

    private TraceTelemetryBuilder(string category, SeverityLevel severityLevel, string message)
    {
        _category = category;
        _severityLevel = severityLevel;
        _message = message;
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

    public static TraceTelemetry AsFunctionStarted()
    {
        return new TraceTelemetryBuilder(
                DefaultCategory,
                SeverityLevel.Information,
                $"Executing '{DefaultFunctionName}' (Reason='This function was programmatically called via the host APIs.', Id={DefaultInvocationId})")
            .WithEventId(FunctionRuntimeEventId.FunctionStarted)
            .WithEventName(FunctionRuntimeEventName.FunctionStarted)
            .Build();
    }

    public static TraceTelemetry AsFunctionCompletedSucceeded()
    {
        return new TraceTelemetryBuilder(
                DefaultCategory,
                SeverityLevel.Information,
                $"Executed '{DefaultFunctionName}' (Succeeded, Id={DefaultInvocationId}, Duration=32ms)")
            .WithEventId(FunctionRuntimeEventId.FunctionCompletedSucceeded)
            .WithEventName(FunctionRuntimeEventName.FunctionCompleted)
            .Build();
    }

    public static TraceTelemetry AsFunctionCompletedFailed()
    {
        return new TraceTelemetryBuilder(
                DefaultCategory,
                SeverityLevel.Error,
                $"Executed '{DefaultFunctionName}' (Failed, Id={DefaultInvocationId}, Duration=49ms)")
            .WithEventId(FunctionRuntimeEventId.FunctionCompletedFailed)
            .WithEventName(FunctionRuntimeEventName.FunctionCompleted)
            .Build();
    }

    public static TraceTelemetry AsServiceBusTrigger()
    {
        var traceTelemetry = new TraceTelemetryBuilder(
                DefaultCategory,
                SeverityLevel.Information,
                "Trigger Details: MessageId: 9e778a1d49c04c72bd7e78c60c10daa0, SequenceNumber: 8, DeliveryCount: 1, EnqueuedTimeUtc: 2022-03-12T03:11:17.5570000+00:00, LockedUntilUtc: 2022-03-12T03:11:47.5730000+00:00, SessionId: (null)")
            .Build();
        traceTelemetry.Properties["prop__{OriginalFormat}"] =
            "Trigger Details: MessageId: {MessageId}, SequenceNumber: {SequenceNumber}, DeliveryCount: {DeliveryCount}, EnqueuedTimeUtc: {EnqueuedTimeUtc}, LockedUntilUtc: {LockedUntilUtc}, SessionId: {SessionId}";
        return traceTelemetry;
    }

    public static TraceTelemetry AsServiceBusBindingMessageProcessingError()
    {
        return new TraceTelemetryBuilder(
                FunctionRuntimeCategory.ServiceBusListener,
                SeverityLevel.Error,
                "Message processing error (Action=ProcessMessageCallback, EntityPath=custom-exception-queue, Endpoint=prefixsb.servicebus.windows.net)")
            .Build();
    }

    private TraceTelemetry Build()
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
