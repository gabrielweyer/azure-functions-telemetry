using Microsoft.ApplicationInsights.DataContracts;

namespace Custom.FunctionsTelemetry.TestInfrastructure.Builders;

public class DependencyTelemetryBuilder
{
    private readonly string _type;

    public DependencyTelemetryBuilder(string type)
    {
        _type = type;
    }

    public DependencyTelemetry Build()
    {
        var dependencyTelemetry = new DependencyTelemetry();
        dependencyTelemetry.Type = _type;
        return dependencyTelemetry;
    }
}