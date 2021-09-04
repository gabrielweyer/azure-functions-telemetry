using Custom.FunctionsTelemetry.ApplicationInsights;
using Custom.FunctionsTelemetry.Logging;
using Custom.FunctionsTelemetryTests.TestInfrastructure.Builders;
using Custom.FunctionsTelemetryTests.TestInfrastructure.Mocks;
using Microsoft.ApplicationInsights.DataContracts;
using Xunit;

namespace Custom.FunctionsTelemetryTests.ApplicationInsights
{
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
            var traceTelemetry = new TraceTelemetryBuilder(FunctionRuntimeCategory.HostExecutor)
                .WithSeverityLevel(SeverityLevel.Error)
                .WithMessage("Message processing error (Action=UserCallback, ClientId=MessageReceiver1custom-queue, EntityPath=custom-queue, Endpoint=prefixsb.servicebus.windows.net)")
                .Build();

            // Act
            _target.Process(traceTelemetry);

            // Assert
            Assert.False(_innerProcessor.WasProcessorCalled);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GivenHostExecutorTrace_WhenNoMessage_ThenKeepTelemetry(string message)
        {
            // Arrange
            var traceTelemetry = new TraceTelemetryBuilder(FunctionRuntimeCategory.HostExecutor)
                .WithSeverityLevel(SeverityLevel.Error)
                .WithMessage(message)
                .Build();

            // Act
            _target.Process(traceTelemetry);

            // Assert
            Assert.True(_innerProcessor.WasProcessorCalled);
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
}