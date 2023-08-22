# Azure Functions Telemetry

The Application Insights integration for Azure Functions `v3` and `v4` suffers from a few quirks that can lead to a huge Application Insights bill:

- Telemetry processors are not supported, preventing developers from discarding telemetry items
- Each Function execution records a trace when starting and on completion
- Exceptions are logged twice for the HTTP binding
- Exceptions are logged three times for the Service Bus binding

The next issue has no impact on the cost of Application Insights but is related to the development experience. `TelemetryConfiguration` is not registered in the Inversion Of Control container when the Application Insights connection string is not set. Emitting a custom metric requires injecting the `TelemetryConfiguration`. Running locally without having configured the Application Insights connection string will then result in an exception.

The last issue is not related to Application Insights but also negatively affects developers' productivity. The custom Console logger provider used by the Azure Functions runtime does not include the stack trace when displaying an exception (for the HTTP binding at least).

If you are not familiar with some of the more advanced features of Application Insights, I suggest you go through the below references before reading the rest of this document:

- [Telemetry processors][telemetry-processors]
- [Telemetry initializers][telemetry-initializers]

In this repository I have tried to address all the quirks listed above.

:rotating_light: The customisation is experimental. There may be undesired side effects around performance and missing telemetry. If you are experiencing issues such as missing telemetry, no exception being recorded when something does not work as expected I recommend disabling the custom integration and reproducing the problem without it. This can be done by creating an application setting named `DisableApplicationInsightsCustomisation` with a value of `true`.

## Table of contents

- [Using the Application Insights customisation](#using-the-application-insights-customisation)
- [What do I get?](#what-do-i-get)
- [Configuration](#configuration)
- [Migration guides](#migration-guides)
- [Demo](#demo)
- [Q&A](#qa)

## Using the Application Insights customisation

The customisation supports both `v3` and `v4` runtime.

[![Build Status][github-actions-shield]][github-actions] [![NuGet][nuget-tool-badge]][nuget-tool-command]

```powershell
dotnet add package AzureFunctions.Better.ApplicationInsights
```

For the most basic integration, you need to provide:

- `{ApplicationName}` used to set Application Insights' _Cloud role name_ (optional). When not provided, the default behaviour is preserved (the _Cloud role name_ will be set to the Function App's name)
- `{TypeFromEntryAssembly}` typically would be `typeof(Startup)`. When `{ApplicationName}` is provided, I read the [Assembly Informational Version][assembly-informational-version] of the entry assembly to set Application Insights' _Application version_ (I use _unknown_ as a fallback). When `{ApplicationName}` is not provided, _Application version_ will not be present on the telemetry items

In your `Startup` `class` add the below snippet:

```csharp
var appInsightsOptions = new CustomApplicationInsightsOptionsBuilder(
        "{ApplicationName}",
        {TypeFromEntryAssembly})
    .Build();

builder.Services
    .AddCustomApplicationInsights(appInsightsOptions)
    .AddCustomConsoleLogging();
```

## What do I get?

### Discarding Function execution traces

This is implemented by [FunctionExecutionTracesFilter][function-execution-traces-filter] and enabled by default.

This can be disabled by setting the application setting `ApplicationInsights:DiscardFunctionExecutionTraces` to `true`.

### Discarding duplicate exceptions

This is implemented by [DuplicateExceptionsFilter][duplicate-exceptions-filter] and always enabled.

### Discarding health requests

This is enabled by setting the application setting `ApplicationInsights:HealthCheckFunctionName` and supplying the Function name (the argument provided to the `FunctionNameAttribute`). The telemetry processor used is [HealthRequestFilter][health-request-filter].

### Better Service Bus binding "request"

The _URL_ and _response code_ are not being set on the service bus triggered "requests". The [ServiceBusRequestInitializer][service-bus-request-initializer] will do this for you.

- URL: I use the Function name
- Response code: `200` in case of success, `500` in case of failure

The `ServiceBusRequestInitializer` is always enabled.

### Discarding Service Bus trigger traces

This is recommended on high-volume services. This is disabled by default.

This is done by setting the application setting `ApplicationInsights:DiscardServiceBusTrigger` to `true`. The telemetry processor used is [ServiceBusTriggerFilter][service-bus-trigger-filter].

### Replacing the Console logging provider

This is done by calling `AddCustomConsoleLogging`. You will then consistently get stack traces in the console.

### Registering telemetry initializers

:memo: The built-in integration supports telemetry initializers. The custom integration supports registering telemetry initializers in the same way as the built-in integration does.

Telemetry initializers can either be registered using `TImplementation`:

```csharp
builder.Services.AddSingleton<ITelemetryInitializer, YourInitializer>();
```

Or an instance of the telemetry initializer:

```csharp
// Use:
builder.Services.AddSingleton<ITelemetryInitializer>(new YourOtherInitializer("NiceValue"));
// Do not use, otherwise your telemetry initializer will not be called:
builder.Services.AddSingleton(new YourOtherInitializer("NiceValue"));
```

You can add as many telemetry initializers as you want.

### Telemetry processors support

:memo: The built-in integration does **not** support telemetry processors. I have added support so that you can use the same extension method than when registering a telemetry processor in ASP.NET Core:

```csharp
builder.Services.AddApplicationInsightsTelemetryProcessor<YourTelemetryProcessor>();
```

You can add as many telemetry processors as you want.

## Configuration

The customisation is configured using application settings:

```json
{
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "DisableApplicationInsightsCustomisation": true, // Allows you to disable the customisation if you think something is wrong with it
    "ApplicationInsights:DiscardServiceBusTrigger": true,
    "ApplicationInsights:HealthCheckFunctionName": "HealthFunction", // The name of the Function, not the route
    "ApplicationInsights:DiscardFunctionExecutionTraces": false // Allows you to stop discarding Function execution traces
  }
}
```

If you don't want to use the `ApplicationInsights` key, you can provide another value when configuring the customisation:

```csharp
var appInsightsOptions = new CustomApplicationInsightsOptionsBuilder(
        "{ApplicationName}",
        {TypeFromEntryAssembly})
    .WithConfigurationSectionName("SomeOtherSectionName")
    .Build();
```

## Migration guides

- [Migrating from v1 to v2][migrating-from-v1-v2]

## Demo

A [demo](/docs/DEMO.md) demonstrates all the customisation's features.

## Q&A

### Why so much code to support telemetry processors?

There is an [opened GitHub issue][telemetry-processor-support-github-issue] about the lack of telemetry processors support in Azure Functions. The thread supplies a workaround to enable telemetry processors, but the telemetry processors added in this fashion will not be called for request telemetry items.

## Appendix

### Software versions

The latest version of the Azure Functions Core Tools I have been using is `4.0.5198`.

NuGet packages:

- `Microsoft.NET.Sdk.Functions`:
  - `v3`: `3.1.2` (added automatically when creating the Function, updated later)
  - `v4`: `4.1.3` (added automatically when creating the Function, updated later)
- `Microsoft.Azure.Functions.Extensions`: `1.1.0` (added manually following [Use dependency injection in .NET Azure Functions][dependency-injection])
- `Microsoft.Extensions.DependencyInjection` (added manually following [Use dependency injection in .NET Azure Functions][dependency-injection], updated later):
  - `v3`: `3.1.32`
  - `v4`: `6.0.1`
- `Microsoft.Azure.WebJobs.Logging.ApplicationInsights`: `3.0.35` (added manually following [Log custom telemetry in C# Azure Functions][custom-telemetry])

### Supporting telemetry processors

The code in `AddCustomApplicationInsights` retrieves the configured built-in telemetry processors, adds them to a new telemetry processor chain and builds the chain. This gives me the opportunity to add our own processors to the chain.

The first built-in processor in the chain is `OperationFilteringTelemetryProcessor`, this processor discards all the dependencies considered internal to the Azure Functions runtime (such as access to blob storage for the distributed lock and the calls to Azure Service Bus for the Service Bus binding).

One of the side-effects of the approach I am using is that the Azure Functions runtime will reference the initial instance of `OperationFilteringTelemetryProcessor` and will call it directly when tracking requests manually. Normally the `OperationFilteringTelemetryProcessor` instance points to the second processor in the chain (`QuickPulseTelemetryProcessor`). One way for our processors to be called is to point the existing `OperationFilteringTelemetryProcessor` instance to our first processor and point our last processor to `QuickPulseTelemetryProcessor`. This is done through some pretty dodgy untested code, but it works :tm:.

As I did not manage to cover my customisation with unit tests, I wrote [integration tests][integration-tests] instead.

[dependency-injection]: https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection
[custom-telemetry]: https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library?tabs=v4%2Ccmd#log-custom-telemetry-in-c-functions
[telemetry-processors]: https://learn.microsoft.com/en-us/azure/azure-monitor/app/api-filtering-sampling#filtering
[telemetry-initializers]: https://learn.microsoft.com/en-us/azure/azure-monitor/app/api-filtering-sampling#addmodify-properties-itelemetryinitializer
[assembly-informational-version]: https://learn.microsoft.com/en-us/dotnet/standard/assembly/versioning#assembly-informational-version
[function-execution-traces-filter]: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/src/AzureFunctionsTelemetry/ApplicationInsights/FunctionExecutionTracesFilter.cs
[duplicate-exceptions-filter]: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/src/AzureFunctionsTelemetry/ApplicationInsights/DuplicateExceptionsFilter.cs
[health-request-filter]: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/src/AzureFunctionsTelemetry/ApplicationInsights/HealthRequestFilter.cs
[service-bus-request-initializer]: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/src/AzureFunctionsTelemetry/ApplicationInsights/ServiceBusRequestInitializer.cs
[service-bus-trigger-filter]: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/src/AzureFunctionsTelemetry/ApplicationInsights/ServiceBusTriggerFilter.cs
[telemetry-processor-support-github-issue]: https://github.com/Azure/azure-functions-host/issues/3741
[integration-tests]: CONTRIBUTING.md
[github-actions]: https://github.com/gabrielweyer/azure-functions-telemetry/actions/workflows/build.yml
[github-actions-shield]: https://github.com/gabrielweyer/azure-functions-telemetry/actions/workflows/build.yml/badge.svg
[nuget-tool-badge]: https://img.shields.io/nuget/v/AzureFunctions.Better.ApplicationInsights.svg?label=NuGet
[nuget-tool-command]: https://www.nuget.org/packages/AzureFunctions.Better.ApplicationInsights
[migrating-from-v1-v2]: docs/Migrate-from-v1-to-v2.md
