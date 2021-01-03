using CustomApi.Infrastructure.Telemetry;
using CustomApiTests.TestInfrastructure.Mocks;
using Microsoft.ApplicationInsights.DataContracts;
using Xunit;

namespace CustomApiTests.Infrastructure.Telemetry
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

            var traceTelemetry = new TraceTelemetry();
            traceTelemetry.Properties["EventId"] = "1";
            traceTelemetry.Properties["EventName"] = "FunctionStarted";

            // Act

            _target.Process(traceTelemetry);

            // Assert

            Assert.False(_innerProcessor.WasProcessorCalled);
        }

        [Fact]
        public void GivenFunctionCompletedTrace_ThenFilterOutTelemetry()
        {
            // Arrange

            var traceTelemetry = new TraceTelemetry();
            traceTelemetry.Properties["EventId"] = "2";
            traceTelemetry.Properties["EventName"] = "FunctionCompleted";

            // Act

            _target.Process(traceTelemetry);

            // Assert

            Assert.False(_innerProcessor.WasProcessorCalled);
        }

        [Fact]
        public void GivenFunctionCompletedWithErrorTrace_ThenFilterOutTelemetry()
        {
            // Arrange

            var traceTelemetry = new TraceTelemetry();
            traceTelemetry.Properties["EventId"] = "3";
            traceTelemetry.Properties["EventName"] = "FunctionCompleted";

            // Act

            _target.Process(traceTelemetry);

            // Assert

            Assert.False(_innerProcessor.WasProcessorCalled);
        }

        [Fact]
        public void GivenServiceBusBindingMessageProcessingErrorTrace_ThenFilterOutTelemetry()
        {
            // Arrange

            var traceTelemetry = new TraceTelemetry();
            traceTelemetry.SeverityLevel = SeverityLevel.Error;
            traceTelemetry.Message = "Message processing error (Action=UserCallback, ClientId=MessageReceiver1custom-queue, EntityPath=custom-queue, Endpoint=prefixsb.servicebus.windows.net)";
            traceTelemetry.Properties["Category"] = "Host.Executor";

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

            var traceTelemetry = new TraceTelemetry();
            traceTelemetry.SeverityLevel = SeverityLevel.Error;
            traceTelemetry.Message = message;
            traceTelemetry.Properties["Category"] = "Host.Executor";

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