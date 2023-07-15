# Better Azure Functions Application Insights integration

Automatically discards redundant telemetry items such as Functions execution traces and duplicate exceptions. Allows you to register your own telemetry processor(s) that will be invoked on every telemetry item type.

A more detailed documentation is available on [GitHub][documentation].

## Usage

In your `Startup` `class` add the below snippet:

```csharp
var appInsightsOptions = new CustomApplicationInsightsOptionsBuilder(
        "{ApplicationName}",
        {TypeFromEntryAssembly})
    .Build();

builder.Services
    .AddCustomApplicationInsights(appInsightsOptions);
```

Where

- `{ApplicationName}` is used to set Application Insights' _Cloud role name_
- `{TypeFromEntryAssembly}` typically would be `typeof(Startup)`. I read the [Assembly Informational Version][assembly-informational-version] of the entry assembly to set Application Insights' _Application version_ (I use _unknown_ as a fallback)

Additionaly you can discard health requests and Service Bus trigger traces using application settings:

```json
{
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "ApplicationInsights:DiscardServiceBusTrigger": true,
    "ApplicationInsights:HealthCheckFunctionName": "HealthFunction" // The name of the Function, not the route
  }
}
```

You can also add telemetry initializers and telelemetry processors:

```csharp
public override void Configure(IFunctionsHostBuilder builder)
{
    var appInsightsOptions = new CustomApplicationInsightsOptionsBuilder(
            "customv4inprocess", // Will be used as Cloud role name
            typeof(Startup)) // Assembly Informational Version will be used as Application version
        .Build();

    builder.Services
        // Telemetry processors are supported on all telemetry item types
        .AddApplicationInsightsTelemetryProcessor<CustomHttpDependencyFilter>()
        .AddSingleton<ITelemetryInitializer, TelemetryCounterInitializer>()
        /*
         * When adding an instance of a telemetry initializer, you need to provide the service Type otherwise
         * your initializer will not be used.
         *
         * <code>
         * // Do not use:
         * .AddSingleton(new TelemetryCounterInstanceInitializer("NiceValue"))
         * </code>
         */
        .AddSingleton<ITelemetryInitializer>(new TelemetryCounterInstanceInitializer("NiceValue"))
        .AddCustomApplicationInsights(appInsightsOptions);
}
```

## Migration guides

- [Migrating from v1 to v2][migrating-from-v1-v2]

## Release notes

Release notes can be found on [GitHub][release-notes].

## Feedback

Leave feedback by [opening an issue][open-issue] on GitHub.

[assembly-informational-version]: https://docs.microsoft.com/en-us/dotnet/standard/assembly/versioning#assembly-informational-version
[release-notes]: https://github.com/gabrielweyer/azure-functions-telemetry/releases
[documentation]: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/README.md
[open-issue]: https://github.com/gabrielweyer/azure-functions-telemetry/issues/new
[migrating-from-v1-v2]: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/docs/Migrate-from-v1-to-v2.md
