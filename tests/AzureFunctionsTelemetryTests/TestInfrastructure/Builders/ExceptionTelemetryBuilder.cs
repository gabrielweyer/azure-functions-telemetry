using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs.Logging;

namespace Gabo.AzureFunctionsTelemetryTests.TestInfrastructure.Builders;

public class ExceptionTelemetryBuilder
{
    private readonly string _category;
    private string _eventId;
    private string _eventName;

    public ExceptionTelemetryBuilder(string category)
    {
        _category = category;
    }

    private ExceptionTelemetryBuilder WithEventId(string eventId)
    {
        _eventId = eventId;
        return this;
    }

    private ExceptionTelemetryBuilder WithEventName(string eventName)
    {
        _eventName = eventName;
        return this;
    }

    public static ExceptionTelemetry AsFunctionCompletedFailed(string category)
    {
        return new ExceptionTelemetryBuilder(category)
            .WithEventId(FunctionRuntimeEventId.FunctionCompletedFailed)
            .WithEventName(FunctionRuntimeEventName.FunctionCompleted)
            .Build();
    }

    public ExceptionTelemetry Build()
    {
        var exceptionTelemetry = new ExceptionTelemetry();
        exceptionTelemetry.Properties[LogConstants.CategoryNameKey] = _category;
        exceptionTelemetry.Properties[LogConstants.EventIdKey] = _eventId;
        exceptionTelemetry.Properties[LogConstants.EventNameKey] = _eventName;
        return exceptionTelemetry;
    }
}