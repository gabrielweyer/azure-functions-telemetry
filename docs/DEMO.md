# Demo

The demo requires an Azure Service Bus namespace to run. Functions can run both locally and in Azure, when running locally I recommend stopping the Functions deployed in Azure otherwise the Service Bus triggers will compete for messages.

Before being able to deploy and run the Functions you will need to have the below software installed:

- [Azurite][azurite] is used as the Azure blob storage emulator when running locally
- [Azure Functions Core Tools v4][azure-functions-core-tools] if you want to run from the command line (you will need the `v4` version if you want to be able to run the `v4` Functions locally)
- [Powershell 7][powershell-7] to deploy to Azure
- [Azure PowerShell][azure-powershell] to deploy to Azure
- [Bicep CLI][bicep-cli] to deploy to Azure

Run `Deploy.ps1` to deploy the project to Azure. This will deploy:

- A Workspace based Application Insights instance
- A Service Bus namespace
- Four Function Apps and their supporting storage accounts

```powershell
.\Deploy.ps1 -Location {AzureRegion} -ResourceNamePrefix {UniquePrefix}
```

The project contains four Functions Apps:

- `DefaultV3InProcessFunction` and `DefaultV4InProcessFunction` demonstrate the quirks of In-Process Azure Functions `v3` / `v4` Application Insights integration
- `CustomV3InProcessFunction` and `CustomV4InProcessFunction` demonstrate the workarounds I use to improve In-Process Azure Functions `v3` / `v4` Application Insights integration

I've decided to commit the `local.settings.json` file. This is **not the default or recommended approach** but it makes it easier for new joiners to get started.

You can start the Function Apps by issuing the below commands from the root of repository:

```powershell
cd .\samples\DefaultV3InProcessFunction\
func start
```

```powershell
cd .\samples\DefaultV4InProcessFunction\
func start
```

```powershell
cd .\samples\CustomV3InProcessFunction\
func start
```

```powershell
cd .\samples\CustomV4InProcessFunction\
func start
```

The Function Apps run on fixed ports locally so that you can run all four Functions at the same time:

- Default `v3`: `7071`
- Default `v4`: `7073`
- Custom `v3`: `7072`
- Custom `v4`: `7074`

You can call the different endpoints using this [Postman collection][postman-collection].

## AvailabilityFunction

Navigate to `http://localhost:7074/availability` (Custom `v4`) in your favourite browser.

Emits an availability telemetry item. This is normally emitted by tooling such as Application Insights [URL ping test][url-ping-test]. The reason I'm emitting it manually is to demonstrate that the telemetry processors are called for availability telemetry items.

## CustomEventFunction

You'll need to delete the Application Insights connection string secret in order to reproduce the error. Stop all the Functions and then run:

```powershell
dotnet user-secrets list --id 074ca336-270b-4832-9a1a-60baf152b727
```

Make a note of the value of the `APPLICATIONINSIGHTS_CONNECTION_STRING` secret, then delete it:

```powershell
dotnet user-secrets remove APPLICATIONINSIGHTS_CONNECTION_STRING `
    --id 074ca336-270b-4832-9a1a-60baf152b727
```

Navigate to `http://localhost:7073/event` (Default `v4`) in your favourite browser.

Demonstrate that when the secret `APPLICATIONINSIGHTS_CONNECTION_STRING` is not set, attempting to retrieve `TelemetryConfiguration` from the container results in an exception:

![Without the secret `APPLICATIONINSIGHTS_CONNECTION_STRING`, `TelemetryConfiguration` is not registered and an exception is thrown](img/telemetry-configuration-not-registered.png)

:memo: When using ASP.NET Core, `TelemetryConfiguration` is registered by calling `AddApplicationInsightsTelemetry()` in `Startup.cs` but this method [should not be called in Azure Functions][dont-call-add-app-insights-telemetry]:

> Don't add `AddApplicationInsightsTelemetry()` to the services collection, which registers services that conflict with services provided by the environment.

Navigate to `http://localhost:7074/event` (Custom `v4`) in your favourite browser.

Demonstrate that when the secret `APPLICATIONINSIGHTS_CONNECTION_STRING` is not set, attempting to retrieve `TelemetryConfiguration` from the container does not result in an exception because I [register a no-op TelemetryConfiguration][default-telemetry-configuration-registration] if one was not registered already:

![Without the secret `APPLICATIONINSIGHTS_CONNECTION_STRING`, `TelemetryConfiguration` is registered and no exception is thrown](img/telemetry-configuration-registered.png)

Finally once done, you can add the secret again:

```powershell
dotnet user-secrets set APPLICATIONINSIGHTS_CONNECTION_STRING '{YourConnectionString}' `
    --id 074ca336-270b-4832-9a1a-60baf152b727
```

## DependencyFunction

Navigate to `http://localhost:7073/dependency` (Default `v4`) in your favourite browser.

Four telemetry items are recorded:

- The request itself
- The _Executing ..._ and _Executed ..._ traces
- The custom dependency we've manually tracked

![Dependency Function default telemetry](img/dependency-function-default.png)

Navigate to `http://localhost:7074/dependency` (Custom `v4`) in your favourite browser.

Only the request is recorded:

- The _Executing ..._ and _Executed ..._ traces have been discarded by the `FunctionExecutionTracesFilter`
- The custom dependency we've manually tracked has been discarded by the `CustomHttpDependencyFilter`

![Dependency Function custom telemetry](img/dependency-function-custom.png)

The `CustomHttpDependencyFilter` discards a specific telemetry type. This is useful when having a noisy telemetry. You can tweak the processor to only discard successful or fast dependencies.

:rotating_light: Discarding telemetry skews the statistics. Consider using [sampling][adaptive-sampling] instead.

## HealthFunction

To keep Function Apps on a consumption plan alive and limit the number of cold starts, developers tend to use Application Insights [URL ping test][url-ping-test]. This results in many requests being recorded in Application Insights.

:memo: `HEAD` is more commonly used than `GET` for ping tests but it is easier to issue a `GET` with a web browser.

Navigate to `http://localhost:7073/health` (Default `v4`) in your favourite browser.

The Health request is recorded in Application Insights.

Navigate to `http://localhost:7074/health` (Custom `v4`) in your favourite browser.

The Health request is discarded by the `HealthRequestFilter` which is configured by `WithHealthRequestFilter`.

## HttpExceptionThrowingFunction

Navigate to `http://localhost:7073/http-exception` (Default `v4`) in your favourite browser.

Demonstrates that the stack trace is not present in the console logs when an exception is thrown.

![No stack trace in the console when an exception is thrown](img/console-stack-trace-absent.png)

This also proves that the same exception appears twice in Application Insights:

![The same exception is logged twice for the HTTP binding](img/http-binding-exception-logged-twice.png)

Navigate to `http://localhost:7074/http-exception` (Custom `v4`) in your favourite browser.

Demonstrates that the stack trace is present in the console logs when an exception is thrown.

![Stack trace present in the console when an exception is thrown](img/console-stack-trace-present.png)

This also proves that the same exception appears only once in Application Insights:

![The same exception is logged only once for the HTTP binding](img/http-binding-exception-logged-once.png)

## ProcessorFunction

Navigate to `http://localhost:7073/processor` (Default `v4`) in your favourite browser.

Demonstrates that our `TelemetryCounterProcessor` telemetry processor is not being called even though I added it using `AddApplicationInsightsTelemetryProcessor`.

![Our telemetry processor is not being called for requests](img/telemetry-processor-is-not-being-called.png)

Navigate to `http://localhost:7074/processor` (Custom `v4`) in your favourite browser.

Demonstrates that our `TelemetryCounterProcessor` telemetry processor is being called:

![Our telemetry processor is even being called for requests](img/telemetry-counter-is-being-called.png)

Note that the processor is also called for request telemetry items. When running in Azure you might get different results on each request as you might be hitting different instances and the state is kept in-memory.

## ServiceBusFunction

You can send a message to the `defaultv4inprocess-queue` queue using the Service Bus Explorer in the Azure Portal or you can navigate to `http://localhost:7073/service-bus` (Default `v4`) in your favourite browser.

The Default Function does not have a _URL_ or a _Response code_:

![Service Bus binding: no URL and no response code for the Default App](img/service-bus-binding-no-request-url-no-response-code-default.png)

Four telemetry items are recorded for the Default Function execution:

- The request itself
- The _Executing ..._ and _Executed ..._ traces
- The _Trigger Details ..._ trace

![Service Bus binding: default telemetry](img/service-bus-binding-default.png)

You can send a message to the `customv4inprocess-queue` queue using the Service Bus Explorer in the Azure Portal or you can navigate to `http://localhost:7074/service-bus` (Custom `v4`) in your favourite browser.

The Custom Function has both the _Request URL_ and _Response code_ set by `ServiceBusRequestInitializer`:

![Service Bus binding: URL and response code are set for the Custom App](img/service-bus-binding-request-url-and-response-code-custom.png)

Only the request is recorded for the Custom Function execution:

- The _Executing ..._ and _Executed ..._ traces have been discarded by the `FunctionExecutionTracesFilter`
- The _Trigger Details ..._ trace has been discarded by the `ServiceBusTriggerFilter`

## ServiceBusExceptionThrowingFunction

You can send a message to the `defaultv4inprocess-exception-queue` queue using the Service Bus Explorer in the Azure Portal or you can navigate to `http://localhost:7073/service-bus-exception` (Default `v4`) in your favourite browser.

Demonstrate that a single exception thrown by the Function is recorded three times in Application Insights and that a total of nine telemetry items are emitted during the Function execution.

![Service Bus binding: nine telemetry items emitted by the Functions runtime](img/service-bus-binding-execution-nine-telemetry-items.png)

You can send a message to the `customv4inprocess-exception-queue` queue using the Service Bus Explorer in the Azure Portal or you can navigate to `http://localhost:7074/service-bus-exception` (Custom `v4`) in your favourite browser.

Demonstrate that a single exception thrown by the Function is recorded only once in Application Insights and that a total of three telemetry items are emitted during the Function execution.

![Service Bus binding: three telemetry items emitted by the Functions runtime](img/service-bus-binding-execution-three-telemetry-items.png)

## TraceLogFunction

Navigate to `http://localhost:7073/trace-log` (Default `v4`) / `http://localhost:7074/trace-log` (Custom `v4`) in your favourite browser.

Demonstrate that log events are not filtered before being sent to Live Metrics. This is not a limitation of Azure Functions, that's how Application Insights works and something you need to be aware of.

![A `Trace` log event is displayed in the Live Metrics](img/trace-log-live-metrics.png)

## UserSecretFunction

Navigate to `http://localhost:7073/secret` (Default `v4`) / `http://localhost:7074/secret` (Custom `v4`) in your favourite browser.

Demonstrates that Azure Functions can use the [Secret Manager][secret-manager] when running locally.

## Application Version and Cloud Role Name

By default, the _Application Version_ is not set, and the _Cloud Role Name_ will be the Function App Azure resource name:

![Default Cloud Role Name and Application Version](img/cloud-role-name-application-version-default.png)

For the custom Function, each telemetry will be stamped with the Assembly Informational Version and the configured application name:

![Custom Cloud Role Name and Application Version](img/cloud-role-name-application-version-custom.png)

## Discarding SystemTraceMiddleware logs

The `SystemTraceMiddleware` emits two log events per HTTP Function execution when running locally:

![SystemTraceMiddleware: two log events per HTTP Function execution when running locally](img/system-trace-middleware-local-two-logs-http-function-execution.png)

These can be suppressed by adding the below `Value` to `local.settings.json` (not `host.json`):

```json
"logging:logLevel:Microsoft.Azure.WebJobs.Script.WebHost.Middleware.SystemTraceMiddleware": "None"
```

![SystemTraceMiddleware: suppresed logs](img/system-trace-middleware-suppressed-logs.png)

Anthony Chu has [documented how to suppress some logs][anthony-chu-suppress-logs].

[azurite]: https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite
[azure-functions-core-tools]: https://github.com/Azure/azure-functions-core-tools
[powershell-7]: https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-7.2
[azure-powershell]: https://learn.microsoft.com/en-us/powershell/azure/install-az-ps?view=azps-9.4.0
[dont-call-add-app-insights-telemetry]: https://docs.microsoft.com/en-US/azure/azure-functions/functions-dotnet-dependency-injection#logging-services
[secret-manager]: https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#secret-manager
[default-telemetry-configuration-registration]: https://github.com/gabrielweyer/azure-functions-telemetry/blob/81a44091df4263442bb0e205a3942cfa5bfedb70/src/AzureFunctionsTelemetry/ApplicationInsights/ApplicationInsightsServiceCollectionExtensions.cs#L231-L235
[url-ping-test]: https://docs.microsoft.com/en-us/azure/azure-monitor/app/availability-overview
[anthony-chu-suppress-logs]: https://github.com/anthonychu/functions-log-suppression#readme
[postman-collection]: postman/FunctionsTelemetry.postman_collection.json
[bicep-cli]: https://docs.microsoft.com/en-au/azure/azure-resource-manager/bicep/install#install-manually
[adaptive-sampling]: https://docs.microsoft.com/en-us/azure/azure-monitor/app/sampling#adaptive-sampling
