namespace Gabo.AzureFunctionsTelemetryTests.ApplicationInsights;

public class ServiceBusTriggerFilterTests
{
    private readonly ServiceBusTriggerFilter _target;
    private readonly MockTelemetryProcessor _innerProcessor;

    public ServiceBusTriggerFilterTests()
    {
        _innerProcessor = new MockTelemetryProcessor();
        _target = new ServiceBusTriggerFilter(_innerProcessor);
    }

    [Fact]
    public void GivenTriggerTrace_ThenDiscardTelemetry()
    {
        // Arrange
        var traceTelemetry = TraceTelemetryBuilder.AsServiceBusTrigger();

        // Act
        _target.Process(traceTelemetry);

        // Assert
        Assert.False(_innerProcessor.WasProcessorCalled);
    }

    [Fact]
    public void GivenOtherTrace_ThenKeepTelemetry()
    {
        // Arrange
        var traceTelemetry = TraceTelemetryBuilder.AsFunctionStarted();

        // Act
        _target.Process(traceTelemetry);

        // Assert
        Assert.True(_innerProcessor.WasProcessorCalled);
    }
}
