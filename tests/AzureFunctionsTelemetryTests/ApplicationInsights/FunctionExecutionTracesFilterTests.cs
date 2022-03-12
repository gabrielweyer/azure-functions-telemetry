using Microsoft.ApplicationInsights.DataContracts;

namespace Gabo.AzureFunctionsTelemetryTests.ApplicationInsights;

public class FunctionExecutionTracesFilterTests
{
    private readonly FunctionExecutionTracesFilter _target;

    private readonly MockTelemetryProcessor _innerProcessor;

    public FunctionExecutionTracesFilterTests()
    {
        _innerProcessor = new MockTelemetryProcessor();

        _target = new FunctionExecutionTracesFilter(_innerProcessor);
    }

    [Fact]
    public void GivenFunctionStartedTrace_ThenFilterOutTelemetry()
    {
        // Arrange
        var traceTelemetry = TraceTelemetryBuilder.AsFunctionStarted();

        // Act
        _target.Process(traceTelemetry);

        // Assert
        Assert.False(_innerProcessor.WasProcessorCalled);
    }

    [Fact]
    public void GivenFunctionCompletedTrace_ThenFilterOutTelemetry()
    {
        // Arrange
        var traceTelemetry = TraceTelemetryBuilder.AsFunctionCompletedSucceeded();

        // Act
        _target.Process(traceTelemetry);

        // Assert
        Assert.False(_innerProcessor.WasProcessorCalled);
    }

    [Fact]
    public void GivenFunctionCompletedWithErrorTrace_ThenFilterOutTelemetry()
    {
        // Arrange
        var traceTelemetry = TraceTelemetryBuilder.AsFunctionCompletedFailed();

        // Act
        _target.Process(traceTelemetry);

        // Assert
        Assert.False(_innerProcessor.WasProcessorCalled);
    }

    [Fact]
    public void GivenServiceBusBindingMessageProcessingErrorTrace_ThenFilterOutTelemetry()
    {
        // Arrange
        var traceTelemetry = TraceTelemetryBuilder.AsServiceBusBindingMessageProcessingError();

        // Act
        _target.Process(traceTelemetry);

        // Assert
        Assert.False(_innerProcessor.WasProcessorCalled);
    }

    [Fact]
    public void GivenTelemetryIsNotTraceTelemetry_ThenKeepTelemetry()
    {
        // Arrange
        var exceptionTelemetry = new ExceptionTelemetry();

        // Act
        _target.Process(exceptionTelemetry);

        // Assert
        Assert.True(_innerProcessor.WasProcessorCalled);
    }

    [Fact]
    public void GivenTraceTelemetryThatIsNeitherFunctionStartedOrCompleted_ThenKeepTelemetry()
    {
        // Arrange
        var traceTelemetry = new TraceTelemetry();

        // Act
        _target.Process(traceTelemetry);

        // Assert
        Assert.True(_innerProcessor.WasProcessorCalled);
    }
}
