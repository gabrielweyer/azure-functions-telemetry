using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights
{
    internal class ApplicationInitializer : ITelemetryInitializer
    {
        private readonly ApplicationDescriptor _applicationDescriptor;

        public ApplicationInitializer(ApplicationDescriptor applicationDescriptor)
        {
            _applicationDescriptor = applicationDescriptor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = _applicationDescriptor.Name;
            telemetry.Context.Component.Version = _applicationDescriptor.Version;
        }
    }
}