namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

internal class ApplicationDescriptor
{
    public ApplicationDescriptor(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public string Name { get; }
    public string Version { get; }
}