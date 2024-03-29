namespace Gabo.AzureFunctionsTelemetryTests.ApplicationInsights;

public class DuplicateExceptionsFilterHttpBindingTests
{
    private readonly DuplicateExceptionsFilter _target;

    private readonly MockTelemetryProcessor _innerProcessor;

    public DuplicateExceptionsFilterHttpBindingTests()
    {
        _innerProcessor = new MockTelemetryProcessor();

        _target = new DuplicateExceptionsFilter(_innerProcessor, new List<string>());
    }

    [Fact]
    public void GivenCategoryIsHostResultsAsUsedByAllBindings_WhenTelemetryIsException_ThenFilterOutTelemetry()
    {
        // Arrange
        var exceptionTelemetry = new ExceptionTelemetryBuilder(FunctionRuntimeCategory.HostResults)
            .Build();

        // Act
        _target.Process(exceptionTelemetry);

        // Assert
        Assert.False(_innerProcessor.WasProcessorCalled);
    }

    [Fact]
    public void GivenCategoryIsHostExecutorAsUsedByServiceBusBinding_WhenTelemetryIsException_ThenKeepTelemetry()
    {
        // Arrange
        var exceptionTelemetry = new ExceptionTelemetryBuilder(FunctionRuntimeCategory.HostExecutor)
            .Build();

        // Act
        _target.Process(exceptionTelemetry);

        // Assert
        Assert.True(_innerProcessor.WasProcessorCalled);
    }

    [Fact]
    public void GivenFunctionCategory_WhenTelemetryIsException_ThenKeepTelemetry()
    {
        // Arrange
        var exceptionTelemetry = new ExceptionTelemetryBuilder("Function.QueueFunction")
            .Build();

        // Act
        _target.Process(exceptionTelemetry);

        // Assert
        Assert.True(_innerProcessor.WasProcessorCalled);
    }
}
