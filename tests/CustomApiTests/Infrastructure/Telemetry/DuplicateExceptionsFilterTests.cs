using CustomApi.Infrastructure.Telemetry;
using CustomApiTests.TestInfrastructure.Mocks;
using Microsoft.ApplicationInsights.DataContracts;
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

            var exceptionTelemetry = new ExceptionTelemetry();
            exceptionTelemetry.Properties["Category"] = "Host.Results";

            // Act

            _target.Process(exceptionTelemetry);

            // Assert

            Assert.False(_innerProcessor.WasProcessorCalled);
        }

        [Fact]
        public void GivenCategoryIsHostExecutorAsUsedByServiceBusBinding_WhenTelemetryIsException_ThenKeepTelemetry()
        {
            // Arrange

            var exceptionTelemetry = new ExceptionTelemetry();
            exceptionTelemetry.Properties["Category"] = "Host.Executor";

            // Act

            _target.Process(exceptionTelemetry);

            // Assert

            Assert.True(_innerProcessor.WasProcessorCalled);
        }

        [Fact]
        public void GivenFunctionCategory_WhenTelemetryIsException_ThenKeepTelemetry()
        {
            // Arrange

            var exceptionTelemetry = new ExceptionTelemetry();
            exceptionTelemetry.Properties["Category"] = "Function.QueueFunction";

            // Act

            _target.Process(exceptionTelemetry);

            // Assert

            Assert.True(_innerProcessor.WasProcessorCalled);
        }

        [Fact]
        public void GivenCategoryIsHostResults_WhenTelemetryIsNotException_ThenKeepTelemetry()
        {
            // Arrange

            var exceptionTelemetry = new TraceTelemetry();
            exceptionTelemetry.Properties["Category"] = "Host.Executor";

            // Act

            _target.Process(exceptionTelemetry);

            // Assert

            Assert.True(_innerProcessor.WasProcessorCalled);
        }
    }
}
