using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Custom.FunctionsTelemetry.ApplicationInsightsTests.TestInfrastructure.Mocks.@new
{
    public class MockTelemetryProcessor : ITelemetryProcessor
    {
        public bool WasProcessorCalled { get; private set; }

        public void Process(ITelemetry item)
        {
            WasProcessorCalled = true;
        }
    }
}