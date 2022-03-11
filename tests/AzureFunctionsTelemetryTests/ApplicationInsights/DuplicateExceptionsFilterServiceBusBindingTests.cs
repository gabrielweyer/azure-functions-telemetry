namespace Gabo.AzureFunctionsTelemetryTests.ApplicationInsights;

public class DuplicateExceptionsFilterServiceBusBindingTests
{
    private readonly DuplicateExceptionsFilter _target;

    private readonly MockTelemetryProcessor _innerProcessor;

    public DuplicateExceptionsFilterServiceBusBindingTests()
    {
        _innerProcessor = new MockTelemetryProcessor();

        _target = new DuplicateExceptionsFilter(_innerProcessor, new List<string> { "QueueFunction" });
    }

    [Fact]
    public void
        GivenFunctionExecutionEndsInException_WhenFunctionIsKnownToBeServiceBusBinding_ThenFilterOutTelemetry()
    {
        // Arrange
        var exceptionTelemetry = ExceptionTelemetryBuilder
            .AsFunctionCompletedFailed("Function.QueueFunction");

        // Act
        _target.Process(exceptionTelemetry);

        // Assert
        Assert.False(_innerProcessor.WasProcessorCalled);
    }
}
