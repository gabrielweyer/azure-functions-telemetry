using CustomApi.Infrastructure.Telemetry;
using CustomApiTests.TestInfrastructure.Builders;
using CustomApiTests.TestInfrastructure.Mocks;
using Xunit;

namespace CustomApiTests.Infrastructure.Telemetry
{
    public class DuplicateExceptionsFilterTests
    {
        private readonly DuplicateExceptionsFilter _target;

        private readonly MockTelemetryProcessor _innerProcessor;

        public DuplicateExceptionsFilterTests()
        {
            _innerProcessor = new MockTelemetryProcessor();

            _target = new DuplicateExceptionsFilter(_innerProcessor);
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

        [Fact]
        public void GivenCategoryIsHostResults_WhenTelemetryIsNotException_ThenKeepTelemetry()
        {
            // Arrange

            var traceTelemetry = new TraceTelemetryBuilder(FunctionRuntimeCategory.HostResults)
                .Build();

            // Act

            _target.Process(traceTelemetry);

            // Assert

            Assert.True(_innerProcessor.WasProcessorCalled);
        }

        [Fact]
        public void
            GivenFunctionExecutionEndsInException_WhenFunctionIsKnownToBeServiceBusBinding_ThenFilterOutTelemetry()
        {
            var exceptionTelemetry = ExceptionTelemetryBuilder
                .AsFunctionCompletedFailed("Function.QueueFunction");

            // Act

            _target.Process(exceptionTelemetry);

            // Assert

            Assert.False(_innerProcessor.WasProcessorCalled);
        }
    }
}
