using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace DefaultFunction.Infrastructure.Telemetry
{
    public class ApplicationInitializer : ITelemetryInitializer
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

    public class ApplicationDescriptor
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}