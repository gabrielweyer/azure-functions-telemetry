using Custom.FunctionsTelemetry.ApplicationInsights;
using Custom.FunctionsTelemetryTests.TestInfrastructure.Builders;
using Custom.FunctionsTelemetryTests.TestInfrastructure.Mocks.@new;
using Xunit;

namespace Custom.FunctionsTelemetryTests.ApplicationInsights
{
    public class DependencyFilterTests
    {
        private readonly DependencyFilter _target;
        private readonly MockTelemetryProcessor _innerProcessor;

        public DependencyFilterTests()
        {
            _innerProcessor = new MockTelemetryProcessor();
            _target = new DependencyFilter(_innerProcessor, "CustomDependency");
        }

        [Fact]
        public void GivenCustomDependency_ThenFilterOutTelemetry()
        {
            // Arrange
            var dependencyTelemetry = new DependencyTelemetryBuilder("CustomDependency")
                .Build();

            // Act
            _target.Process(dependencyTelemetry);

            // Assert
            Assert.False(_innerProcessor.WasProcessorCalled);
        }

        [Fact]
        public void GivenHttpDependency_ThenKeepTelemetry()
        {
            // Arrange
            var dependencyTelemetry = new DependencyTelemetryBuilder("HTTP")
                .Build();

            // Act
            _target.Process(dependencyTelemetry);

            // Assert
            Assert.True(_innerProcessor.WasProcessorCalled);
        }
    }
}