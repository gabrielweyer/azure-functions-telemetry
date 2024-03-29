using Newtonsoft.Json.Linq;

namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal sealed class TelemetryItemConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotSupportedException("We only use this converter to read.");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var name = (string?)jsonObject["name"];

        TelemetryItem item = name switch
        {
            "AppMetrics" => new MetricItem(),
            "AppTraces" => new TraceItem(),
            "AppRequests" => new RequestItem(),
            "AppExceptions" => new ExceptionItem(),
            "AppDependencies" => new DependencyItem(),
            _ => throw new NotSupportedException($"The telemetry type {name} is not supported.")
        };

        serializer.Populate(jsonObject.CreateReader(), item);

        return item;
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(TelemetryItem).IsAssignableFrom(objectType);
    }
}
