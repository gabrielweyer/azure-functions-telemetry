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

## Release notes

Release notes can be found on [GitHub][release-notes].

[assembly-informational-version]: https://docs.microsoft.com/en-us/dotnet/standard/assembly/versioning#assembly-informational-version
[release-notes]: https://github.com/gabrielweyer/azure-functions-telemetry/releases
[documentation]: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/README.md
