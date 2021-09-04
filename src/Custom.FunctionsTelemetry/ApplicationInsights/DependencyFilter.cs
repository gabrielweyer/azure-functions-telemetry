using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Custom.FunctionsTelemetry.ApplicationInsights
{
    public class DependencyFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;
        private readonly string _dependencyType;

        public DependencyFilter(ITelemetryProcessor next, string dependencyType)
        {
            _next = next;
            _dependencyType = dependencyType;
        }

        public void Process(ITelemetry item)
        {
            if (item is DependencyTelemetry dependency &&
                StringHelper.IsSame(_dependencyType, dependency.Type))
            {
                return;
            }

            _next.Process(item);
        }
    }
}